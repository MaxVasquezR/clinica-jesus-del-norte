using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class CitaService : ICitaService
{
    // Estados que solo asigna el flujo clínico, nunca un PATCH manual.
    private static readonly Dictionary<EstadoCita, string> EstadosDelFlujo = new()
    {
        [EstadoCita.EnTriaje] = "Se asigna al registrar el triaje.",
        [EstadoCita.EnAtencion] = "Se asigna cuando el médico inicia la atención.",
        [EstadoCita.Finalizada] = "Se asigna cuando el médico completa la atención."
    };

    private readonly IRepositorio<CitaMedica> _citas;
    private readonly IRepositorio<HistorialCita> _historial;
    private readonly IRepositorio<Paciente> _pacientes;
    private readonly IRepositorio<Medico> _medicos;
    private readonly IRepositorio<Especialidad> _especialidades;
    private readonly IRepositorio<MedicoEspecialidad> _medicoEspecialidades;
    private readonly IRepositorio<HorarioMedico> _horarios;
    private readonly IRepositorio<Persona> _personas;
    private readonly CitaFlujo _flujo;
    private readonly Secuencias _secuencias;

    public CitaService(
        IRepositorio<CitaMedica> citas,
        IRepositorio<HistorialCita> historial,
        IRepositorio<Paciente> pacientes,
        IRepositorio<Medico> medicos,
        IRepositorio<Especialidad> especialidades,
        IRepositorio<MedicoEspecialidad> medicoEspecialidades,
        IRepositorio<HorarioMedico> horarios,
        IRepositorio<Persona> personas,
        CitaFlujo flujo,
        Secuencias secuencias)
    {
        _citas = citas;
        _historial = historial;
        _pacientes = pacientes;
        _medicos = medicos;
        _especialidades = especialidades;
        _medicoEspecialidades = medicoEspecialidades;
        _horarios = horarios;
        _personas = personas;
        _flujo = flujo;
        _secuencias = secuencias;
    }

    public PagedResult<CitaResponseDto> Listar(int page, int size, EstadoCita? estado, int? medicoId, int? pacienteId)
    {
        var citas = _citas.Find(c =>
                (!estado.HasValue || c.Estado == estado.Value) &&
                (!medicoId.HasValue || c.MedicoId == medicoId.Value) &&
                (!pacienteId.HasValue || c.PacienteId == pacienteId.Value))
            .OrderByDescending(c => c.FechaHoraCita)
            .ToList();

        return Paginador.Crear(citas, page, size, Map);
    }

    public CitaResponseDto ObtenerPorId(int id)
    {
        return Map(_flujo.Obtener(id));
    }

    public CitaResponseDto Crear(CrearCitaDto dto)
    {
        ValidarParticipantes(dto.PacienteId, dto.MedicoId, dto.EspecialidadId);
        ValidarHorario(dto.MedicoId, dto.FechaHoraCita, null);

        var cita = new CitaMedica
        {
            CodigoCita = $"CJN-{DateTime.Now:yyyy}-{_secuencias.Siguiente("cita"):D4}",
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            EspecialidadId = dto.EspecialidadId,
            FechaHoraCita = dto.FechaHoraCita,
            MotivoConsulta = Texto.Limpiar(dto.MotivoConsulta),
            PrecioConsulta = _especialidades.GetById(dto.EspecialidadId)!.PrecioBase,
            Estado = EstadoCita.Solicitada
        };

        return Map(_flujo.Registrar(cita));
    }

    public CitaResponseDto Actualizar(int id, ActualizarCitaDto dto)
    {
        var cita = _flujo.Obtener(id);

        if (cita.Estado is not (EstadoCita.Solicitada or EstadoCita.Confirmada) || cita.FechaHoraLlegada is not null)
            throw new BusinessException("Solo se reprograma una cita Solicitada o Confirmada antes de la llegada del paciente.");

        if (dto.FechaHoraCita.HasValue)
        {
            ValidarHorario(cita.MedicoId, dto.FechaHoraCita.Value, cita.Id);
            cita.FechaHoraCita = dto.FechaHoraCita.Value;
        }

        if (dto.MotivoConsulta is not null)
            cita.MotivoConsulta = Texto.Limpiar(dto.MotivoConsulta);

        return Map(_citas.Update(cita));
    }

    public CitaResponseDto CambiarEstado(int id, CambiarEstadoCitaDto dto)
    {
        var cita = _flujo.Obtener(id);

        if (EstadosDelFlujo.TryGetValue(dto.Estado, out var motivo))
            throw new BusinessException($"El estado {dto.Estado} no se asigna a mano. {motivo}");

        if (dto.Estado == EstadoCita.NoAsistio && cita.FechaHoraLlegada is not null)
            throw new BusinessException("El paciente ya llegó: no se puede marcar como NoAsistio.");

        _flujo.CambiarEstado(cita, dto.Estado, dto.Observacion);
        return Map(cita);
    }

    public CitaResponseDto Cancelar(int id, CancelarCitaDto dto)
    {
        var cita = _flujo.Obtener(id);
        _flujo.CambiarEstado(cita, EstadoCita.Cancelada, dto.ObservacionCancelacion);
        return Map(cita);
    }

    public CitaResponseDto RegistrarLlegada(int id)
    {
        var cita = _flujo.Obtener(id);

        if (cita.Estado != EstadoCita.Confirmada)
            throw new BusinessException($"Solo se registra la llegada de una cita Confirmada (estado actual: {cita.Estado}).");
        if (cita.FechaHoraLlegada is not null)
            throw new ConflictException($"La llegada ya se registró a las {cita.FechaHoraLlegada:HH:mm}.");

        cita.FechaHoraLlegada = DateTime.Now;
        return Map(_citas.Update(cita));
    }

    public IReadOnlyList<HistorialCitaResponseDto> Historial(int id)
    {
        _flujo.Obtener(id);
        return _historial.Find(h => h.CitaId == id).Select(h => new HistorialCitaResponseDto
        {
            Id = h.Id,
            CitaId = h.CitaId,
            EstadoAnterior = h.EstadoAnterior,
            EstadoNuevo = h.EstadoNuevo,
            Observacion = h.Observacion
        }.ConAuditoria(h)).ToList();
    }

    private void ValidarParticipantes(int pacienteId, int medicoId, int especialidadId)
    {
        var paciente = _pacientes.GetById(pacienteId);
        if (paciente is null || !paciente.Activo)
            throw new BusinessException($"El paciente con ID {pacienteId} no existe o está inactivo.");

        var medico = _medicos.GetById(medicoId);
        if (medico is null || !medico.Activo)
            throw new BusinessException($"El médico con ID {medicoId} no existe o está inactivo.");

        var especialidad = _especialidades.GetById(especialidadId);
        if (especialidad is null || !especialidad.Activo)
            throw new BusinessException($"La especialidad con ID {especialidadId} no existe o está inactiva.");

        if (!_medicoEspecialidades.Any(me => me.MedicoId == medicoId && me.EspecialidadId == especialidadId && me.Activo))
            throw new BusinessException($"El médico con ID {medicoId} no atiende la especialidad con ID {especialidadId}.");
    }

    // La cita debe caer dentro de un bloque activo del médico y no cruzarse con otra de sus citas.
    private void ValidarHorario(int medicoId, DateTime fechaHora, int? excluirCitaId)
    {
        if (fechaHora <= DateTime.Now)
            throw new BusinessException("La fecha y hora de la cita debe ser futura.");

        var horario = BuscarHorario(medicoId, fechaHora)
            ?? throw new BusinessException(
                $"El médico no atiende el {Horas.DiaDe(fechaHora)} a las {fechaHora:HH:mm}. Revise su horario en GET /api/medicos/{medicoId}/horarios.");

        var inicio = fechaHora;
        var fin = fechaHora.AddMinutes(horario.DuracionCitaMinutos);
        var cruce = _citas.FirstOrDefault(c =>
            c.MedicoId == medicoId && c.Id != excluirCitaId &&
            c.Estado is not (EstadoCita.Cancelada or EstadoCita.NoAsistio) &&
            inicio < c.FechaHoraCita.AddMinutes(BuscarHorario(medicoId, c.FechaHoraCita)?.DuracionCitaMinutos ?? horario.DuracionCitaMinutos) &&
            fin > c.FechaHoraCita);

        if (cruce is not null)
            throw new ConflictException($"El médico ya tiene la cita {cruce.CodigoCita} a las {cruce.FechaHoraCita:HH:mm} (posible overbooking).");
    }

    private HorarioMedico? BuscarHorario(int medicoId, DateTime fechaHora)
    {
        var dia = Horas.DiaDe(fechaHora);
        var minuto = fechaHora.Hour * 60 + fechaHora.Minute;
        return _horarios.FirstOrDefault(h =>
            h.MedicoId == medicoId && h.Activo && h.DiaSemana == dia &&
            Horas.Minutos(Horas.Parse(h.HoraInicio)) <= minuto &&
            minuto + h.DuracionCitaMinutos <= Horas.Minutos(Horas.Parse(h.HoraFin)));
    }

    private string? NombrePersona(int? personaId) =>
        personaId is int id ? _personas.GetById(id)?.NombreCompleto : null;

    private CitaResponseDto Map(CitaMedica c) => new CitaResponseDto
    {
        Id = c.Id,
        CodigoCita = c.CodigoCita,
        PacienteId = c.PacienteId,
        PacienteNombre = NombrePersona(_pacientes.GetById(c.PacienteId)?.PersonaId),
        MedicoId = c.MedicoId,
        MedicoNombre = NombrePersona(_medicos.GetById(c.MedicoId)?.PersonaId),
        EspecialidadId = c.EspecialidadId,
        EspecialidadNombre = _especialidades.GetById(c.EspecialidadId)?.Nombre,
        FechaHoraCita = c.FechaHoraCita,
        MotivoConsulta = c.MotivoConsulta,
        PrecioConsulta = c.PrecioConsulta,
        Estado = c.Estado,
        FechaHoraLlegada = c.FechaHoraLlegada,
        ObservacionCancelacion = c.ObservacionCancelacion
    }.ConAuditoria(c);
}
