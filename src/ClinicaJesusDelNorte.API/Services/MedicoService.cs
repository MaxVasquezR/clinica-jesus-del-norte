using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class MedicoService : IMedicoService
{
    private readonly IRepositorio<Medico> _medicos;
    private readonly IRepositorio<MedicoEspecialidad> _medicoEspecialidades;
    private readonly IRepositorio<Especialidad> _especialidades;
    private readonly IRepositorio<HorarioMedico> _horarios;
    private readonly IPersonaService _personaService;

    public MedicoService(
        IRepositorio<Medico> medicos,
        IRepositorio<MedicoEspecialidad> medicoEspecialidades,
        IRepositorio<Especialidad> especialidades,
        IRepositorio<HorarioMedico> horarios,
        IPersonaService personaService)
    {
        _medicos = medicos;
        _medicoEspecialidades = medicoEspecialidades;
        _especialidades = especialidades;
        _horarios = horarios;
        _personaService = personaService;
    }

    public PagedResult<MedicoResponseDto> Listar(int page, int size, int? especialidadId)
    {
        var medicos = especialidadId is int e
            ? _medicos.Find(m => _medicoEspecialidades.Any(me => me.MedicoId == m.Id && me.EspecialidadId == e && me.Activo))
            : _medicos.GetAll();
        return Paginador.Crear(medicos, page, size, Map);
    }

    public MedicoResponseDto ObtenerPorId(int id)
    {
        return Map(ObtenerMedico(id));
    }

    public MedicoResponseDto Crear(CrearMedicoDto dto)
    {
        var especialidadIds = ValidarEspecialidades(dto.EspecialidadIds);
        var colegiatura = dto.CodigoColegiatura.Trim();

        if (_medicos.Any(m => m.CodigoColegiatura.Equals(colegiatura, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException($"Ya existe un médico con colegiatura {colegiatura}.");
        }

        var existente = _personaService.BuscarPorDocumento(dto.Persona.TipoDocumento, dto.Persona.NumeroDocumento);
        if (existente is not null && _medicos.Any(m => m.PersonaId == existente.Id))
        {
            throw new ConflictException($"Ya existe un médico con documento {existente.NumeroDocumento}.");
        }

        var persona = _personaService.ObtenerOCrear(dto.Persona);
        var medico = _medicos.Add(new Medico
        {
            PersonaId = persona.Id,
            CodigoColegiatura = colegiatura,
            Rne = Texto.Limpiar(dto.Rne),
            Activo = dto.Activo
        });

        SincronizarEspecialidades(medico.Id, especialidadIds);
        return Map(medico);
    }

    public MedicoResponseDto Actualizar(int id, ActualizarMedicoDto dto)
    {
        var medico = ObtenerMedico(id);
        var especialidadIds = ValidarEspecialidades(dto.EspecialidadIds);

        _personaService.Actualizar(medico.PersonaId, dto.Persona);
        medico.Rne = Texto.Limpiar(dto.Rne);
        medico.Activo = dto.Activo;
        _medicos.Update(medico);

        SincronizarEspecialidades(medico.Id, especialidadIds);
        return Map(medico);
    }

    public void Eliminar(int id)
    {
        var medico = ObtenerMedico(id);
        medico.Activo = false;
        _medicos.Update(medico);
    }

    public IReadOnlyList<HorarioMedicoResponseDto> ListarHorarios(int medicoId)
    {
        ObtenerMedico(medicoId);
        return _horarios.Find(h => h.MedicoId == medicoId)
            .OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio)
            .Select(MapHorario).ToList();
    }

    public HorarioMedicoResponseDto CrearHorario(int medicoId, HorarioMedicoDto dto)
    {
        ObtenerMedico(medicoId);
        ValidarHorario(medicoId, dto, null);

        var horario = new HorarioMedico { MedicoId = medicoId };
        AsignarHorario(horario, dto);
        return MapHorario(_horarios.Add(horario));
    }

    public HorarioMedicoResponseDto ActualizarHorario(int medicoId, int horarioId, HorarioMedicoDto dto)
    {
        var horario = ObtenerHorario(medicoId, horarioId);
        ValidarHorario(medicoId, dto, horarioId);
        AsignarHorario(horario, dto);
        return MapHorario(_horarios.Update(horario));
    }

    public void EliminarHorario(int medicoId, int horarioId)
    {
        var horario = ObtenerHorario(medicoId, horarioId);
        horario.Activo = false;
        _horarios.Update(horario);
    }

    private Medico ObtenerMedico(int id) =>
        _medicos.GetById(id) ?? throw new NotFoundException($"No se encontró el médico con ID {id}.");

    private HorarioMedico ObtenerHorario(int medicoId, int horarioId)
    {
        ObtenerMedico(medicoId);
        var horario = _horarios.GetById(horarioId);
        if (horario is null || horario.MedicoId != medicoId)
            throw new NotFoundException($"El médico {medicoId} no tiene un horario con ID {horarioId}.");
        return horario;
    }

    private List<int> ValidarEspecialidades(IEnumerable<int> ids)
    {
        var distintas = ids.Distinct().ToList();
        foreach (var id in distintas)
        {
            var especialidad = _especialidades.GetById(id);
            if (especialidad is null || !especialidad.Activo)
                throw new BusinessException($"La especialidad con ID {id} no existe o está inactiva.");
        }
        return distintas;
    }

    // Activa las especialidades pedidas y desactiva las demás; nunca borra filas.
    private void SincronizarEspecialidades(int medicoId, List<int> especialidadIds)
    {
        var actuales = _medicoEspecialidades.Find(me => me.MedicoId == medicoId);
        foreach (var vinculo in actuales)
        {
            var debeEstarActivo = especialidadIds.Contains(vinculo.EspecialidadId);
            if (vinculo.Activo != debeEstarActivo)
            {
                vinculo.Activo = debeEstarActivo;
                _medicoEspecialidades.Update(vinculo);
            }
        }

        foreach (var id in especialidadIds.Where(id => actuales.All(me => me.EspecialidadId != id)))
        {
            _medicoEspecialidades.Add(new MedicoEspecialidad { MedicoId = medicoId, EspecialidadId = id, Activo = true });
        }
    }

    private void ValidarHorario(int medicoId, HorarioMedicoDto dto, int? excluirId)
    {
        var inicio = Horas.Minutos(Horas.Parse(dto.HoraInicio));
        var fin = Horas.Minutos(Horas.Parse(dto.HoraFin));

        if (inicio >= fin)
            throw new BusinessException("La hora de inicio debe ser anterior a la hora de fin.");
        if (fin - inicio < dto.DuracionCitaMinutos)
            throw new BusinessException("El bloque debe durar al menos lo mismo que una cita.");
        if (!dto.Activo) return;

        var cruce = _horarios.FirstOrDefault(h =>
            h.MedicoId == medicoId && h.Activo && h.Id != excluirId && h.DiaSemana == dto.DiaSemana &&
            inicio < Horas.Minutos(Horas.Parse(h.HoraFin)) && fin > Horas.Minutos(Horas.Parse(h.HoraInicio)));
        if (cruce is not null)
            throw new ConflictException($"El horario se cruza con otro bloque del médico ({cruce.DiaSemana} {cruce.HoraInicio}-{cruce.HoraFin}).");
    }

    private static void AsignarHorario(HorarioMedico horario, HorarioMedicoDto dto)
    {
        horario.DiaSemana = dto.DiaSemana;
        horario.HoraInicio = dto.HoraInicio;
        horario.HoraFin = dto.HoraFin;
        horario.DuracionCitaMinutos = dto.DuracionCitaMinutos;
        horario.Consultorio = Texto.Limpiar(dto.Consultorio);
        horario.Activo = dto.Activo;
    }

    private MedicoResponseDto Map(Medico m) => new MedicoResponseDto
    {
        Id = m.Id,
        CodigoColegiatura = m.CodigoColegiatura,
        Rne = m.Rne,
        Activo = m.Activo,
        Persona = _personaService.Map(m.PersonaId),
        Especialidades = _medicoEspecialidades.Find(me => me.MedicoId == m.Id)
            .Select(me => new MedicoEspecialidadResponseDto
            {
                Id = me.Id,
                EspecialidadId = me.EspecialidadId,
                EspecialidadNombre = _especialidades.GetById(me.EspecialidadId)?.Nombre ?? string.Empty,
                Activo = me.Activo
            }.ConAuditoria(me))
            .ToList()
    }.ConAuditoria(m);

    private static HorarioMedicoResponseDto MapHorario(HorarioMedico h) => new HorarioMedicoResponseDto
    {
        Id = h.Id,
        MedicoId = h.MedicoId,
        DiaSemana = h.DiaSemana,
        HoraInicio = h.HoraInicio,
        HoraFin = h.HoraFin,
        DuracionCitaMinutos = h.DuracionCitaMinutos,
        Consultorio = h.Consultorio,
        Activo = h.Activo
    }.ConAuditoria(h);
}
