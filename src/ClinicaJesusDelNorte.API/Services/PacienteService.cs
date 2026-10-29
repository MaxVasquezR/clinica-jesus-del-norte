using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class PacienteService : IPacienteService
{
    private readonly IPacienteRepository _repository;

    public PacienteService(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public PagedResult<PacienteResponseDto> Listar(int page, int size)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, 100);

        var all = _repository.GetAll();
        var total = all.Count;
        var items = all
            .Skip(page * size)
            .Take(size)
            .Select(Map)
            .ToList();

        return new PagedResult<PacienteResponseDto>
        {
            Items = items,
            Page = page,
            Size = size,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)size)
        };
    }

    public PacienteResponseDto ObtenerPorId(int id)
    {
        var paciente = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró el paciente con ID {id}.");
        return Map(paciente);
    }

    public PacienteResponseDto Crear(CrearPacienteDto dto)
    {
        var existente = _repository.GetByDocumento(dto.NumeroDocumento);
        if (existente is not null)
        {
            throw new ConflictException($"Ya existe un paciente con documento {dto.NumeroDocumento}.");
        }

        var paciente = new Paciente
        {
            TipoDocumento = dto.TipoDocumento,
            NumeroDocumento = dto.NumeroDocumento.Trim(),
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion,
            TipoSeguro = dto.TipoSeguro,
            NumeroPoliza = dto.NumeroPoliza,
            Activo = dto.Activo
        };

        return Map(_repository.Add(paciente));
    }

    public PacienteResponseDto Actualizar(int id, ActualizarPacienteDto dto)
    {
        var paciente = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró el paciente con ID {id}.");

        paciente.Nombres = dto.Nombres.Trim();
        paciente.Apellidos = dto.Apellidos.Trim();
        paciente.FechaNacimiento = dto.FechaNacimiento;
        paciente.Telefono = dto.Telefono;
        paciente.Email = dto.Email;
        paciente.Direccion = dto.Direccion;
        paciente.TipoSeguro = dto.TipoSeguro;
        paciente.NumeroPoliza = dto.NumeroPoliza;
        paciente.Activo = dto.Activo;

        return Map(_repository.Update(paciente));
    }

    public void Eliminar(int id)
    {
        if (!_repository.SoftDelete(id))
        {
            throw new NotFoundException($"No se encontró el paciente con ID {id}.");
        }
    }

    private static PacienteResponseDto Map(Paciente p) => new()
    {
        Id = p.Id,
        CodigoPaciente = p.CodigoPaciente,
        TipoDocumento = p.TipoDocumento,
        NumeroDocumento = p.NumeroDocumento,
        Nombres = p.Nombres,
        Apellidos = p.Apellidos,
        NombreCompleto = p.NombreCompleto,
        FechaNacimiento = p.FechaNacimiento,
        Telefono = p.Telefono,
        Email = p.Email,
        Direccion = p.Direccion,
        TipoSeguro = p.TipoSeguro,
        NumeroPoliza = p.NumeroPoliza,
        Activo = p.Activo,
        FechaRegistro = p.FechaRegistro
    };
}
