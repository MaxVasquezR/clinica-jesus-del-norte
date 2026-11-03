using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class PacienteService : IPacienteService
{
    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<Aseguradora> _aseguradoras;
    private readonly IPersonaService _personaService;
    private readonly Secuencias _secuencias;

    public PacienteService(
        IRepositorio<Paciente> pacientes,
        IRepositorio<Aseguradora> aseguradoras,
        IPersonaService personaService,
        Secuencias secuencias)
    {
        _pacientes = pacientes;
        _aseguradoras = aseguradoras;
        _personaService = personaService;
        _secuencias = secuencias;
    }

    public PagedResult<PacienteResponseDto> Listar(int page, int size)
    {
        return Paginador.Crear(_pacientes.GetAll(), page, size, Map);
    }

    public PacienteResponseDto ObtenerPorId(int id)
    {
        return Map(ObtenerPaciente(id));
    }

    public PacienteResponseDto Crear(CrearPacienteDto dto)
    {
        ValidarAseguradora(dto.AseguradoraId);

        var existente = _personaService.BuscarPorDocumento(dto.Persona.TipoDocumento, dto.Persona.NumeroDocumento);
        if (existente is not null && _pacientes.Any(p => p.PersonaId == existente.Id))
        {
            throw new ConflictException($"Ya existe un paciente con documento {existente.NumeroDocumento}.");
        }

        var persona = _personaService.ObtenerOCrear(dto.Persona);
        var paciente = new Paciente
        {
            PersonaId = persona.Id,
            CodigoPaciente = $"PAC-{DateTime.Now:yyyy}-{_secuencias.Siguiente("paciente"):D4}",
            AseguradoraId = dto.AseguradoraId,
            NumeroPoliza = dto.AseguradoraId.HasValue ? Texto.Limpiar(dto.NumeroPoliza) : null,
            Alergias = Texto.Limpiar(dto.Alergias),
            Activo = dto.Activo
        };

        return Map(_pacientes.Add(paciente));
    }

    public PacienteResponseDto Actualizar(int id, ActualizarPacienteDto dto)
    {
        var paciente = ObtenerPaciente(id);
        ValidarAseguradora(dto.AseguradoraId);

        _personaService.Actualizar(paciente.PersonaId, dto.Persona);
        paciente.AseguradoraId = dto.AseguradoraId;
        paciente.NumeroPoliza = dto.AseguradoraId.HasValue ? Texto.Limpiar(dto.NumeroPoliza) : null;
        paciente.Alergias = Texto.Limpiar(dto.Alergias);
        paciente.Activo = dto.Activo;

        return Map(_pacientes.Update(paciente));
    }

    public void Eliminar(int id)
    {
        var paciente = ObtenerPaciente(id);
        paciente.Activo = false;
        _pacientes.Update(paciente);
    }

    private Paciente ObtenerPaciente(int id) =>
        _pacientes.GetById(id) ?? throw new NotFoundException($"No se encontró el paciente con ID {id}.");

    private void ValidarAseguradora(int? aseguradoraId)
    {
        if (aseguradoraId is null) return;

        var aseguradora = _aseguradoras.GetById(aseguradoraId.Value);
        if (aseguradora is null || !aseguradora.Activo)
            throw new BusinessException($"La aseguradora con ID {aseguradoraId} no existe o está inactiva.");
    }

    private PacienteResponseDto Map(Paciente p) => new PacienteResponseDto
    {
        Id = p.Id,
        CodigoPaciente = p.CodigoPaciente,
        AseguradoraId = p.AseguradoraId,
        AseguradoraNombre = p.AseguradoraId is int a ? _aseguradoras.GetById(a)?.Nombre : null,
        NumeroPoliza = p.NumeroPoliza,
        Alergias = p.Alergias,
        Activo = p.Activo,
        Persona = _personaService.Map(p.PersonaId)
    }.ConAuditoria(p);
}
