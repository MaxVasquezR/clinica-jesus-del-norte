using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IEspecialidadRepository _especialidadRepository;

    private static readonly Dictionary<EstadoCita, HashSet<EstadoCita>> TransicionesPermitidas = new()
    {
        [EstadoCita.Solicitada] = [EstadoCita.Confirmada, EstadoCita.Cancelada],
        [EstadoCita.Confirmada] = [EstadoCita.EnTriaje, EstadoCita.Cancelada, EstadoCita.NoAsistio],
        [EstadoCita.EnTriaje] = [EstadoCita.EnAtencion, EstadoCita.Cancelada],
        [EstadoCita.EnAtencion] = [EstadoCita.Finalizada, EstadoCita.Cancelada],
        [EstadoCita.Finalizada] = [],
        [EstadoCita.Cancelada] = [],
        [EstadoCita.NoAsistio] = []
    };

    public CitaService(
        ICitaRepository citaRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository,
        IEspecialidadRepository especialidadRepository)
    {
        _citaRepository = citaRepository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository = medicoRepository;
        _especialidadRepository = especialidadRepository;
    }

    public PagedResult<CitaResponseDto> Listar(int page, int size, EstadoCita? estado, int? medicoId, int? pacienteId)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, 100);

        var query = _citaRepository.GetAll().AsEnumerable();

        if (estado.HasValue)
            query = query.Where(c => c.Estado == estado.Value);
        if (medicoId.HasValue)
            query = query.Where(c => c.MedicoId == medicoId.Value);
        if (pacienteId.HasValue)
            query = query.Where(c => c.PacienteId == pacienteId.Value);

        var filtered = query.ToList();
        var total = filtered.Count;
        var items = filtered.Skip(page * size).Take(size).Select(Map).ToList();

        return new PagedResult<CitaResponseDto>
        {
            Items = items,
            Page = page,
            Size = size,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)size)
        };
    }

    public CitaResponseDto ObtenerPorId(int id)
    {
        var cita = ObtenerCita(id);
        return Map(cita);
    }

    public CitaResponseDto Crear(CrearCitaDto dto)
    {
        ValidarParticipantes(dto.PacienteId, dto.MedicoId, dto.EspecialidadId);

        if (dto.FechaHoraCita <= DateTime.Now)
        {
            throw new BusinessException("La fecha y hora de la cita debe ser futura.");
        }

        if (_citaRepository.ExistsOverlap(dto.MedicoId, dto.FechaHoraCita))
        {
            throw new ConflictException("El médico ya tiene una cita en ese horario (posible overbooking).");
        }

        var cita = new CitaMedica
        {
            PacienteId = dto.PacienteId,
            MedicoId = dto.MedicoId,
            EspecialidadId = dto.EspecialidadId,
            FechaHoraCita = dto.FechaHoraCita,
            MotivoConsulta = dto.MotivoConsulta,
            Estado = EstadoCita.Confirmada
        };

        return Map(_citaRepository.Add(cita));
    }

    public CitaResponseDto Actualizar(int id, ActualizarCitaDto dto)
    {
        var cita = ObtenerCita(id);

        if (cita.Estado is EstadoCita.Finalizada or EstadoCita.Cancelada)
        {
            throw new BusinessException($"No se puede modificar una cita en estado {cita.Estado}.");
        }

        if (dto.FechaHoraCita.HasValue)
        {
            if (dto.FechaHoraCita.Value <= DateTime.Now)
            {
                throw new BusinessException("La fecha y hora de la cita debe ser futura.");
            }

            if (_citaRepository.ExistsOverlap(cita.MedicoId, dto.FechaHoraCita.Value, cita.Id))
            {
                throw new ConflictException("El médico ya tiene una cita en ese horario.");
            }

            cita.FechaHoraCita = dto.FechaHoraCita.Value;
        }

        if (dto.MotivoConsulta is not null)
            cita.MotivoConsulta = dto.MotivoConsulta;

        if (dto.FechaHoraLlegada.HasValue)
            cita.FechaHoraLlegada = dto.FechaHoraLlegada;

        if (dto.Estado.HasValue)
        {
            AplicarTransicion(cita, dto.Estado.Value, dto.ObservacionCancelacion);
        }

        return Map(_citaRepository.Update(cita));
    }

    public CitaResponseDto CambiarEstado(int id, CambiarEstadoCitaDto dto)
    {
        var cita = ObtenerCita(id);
        AplicarTransicion(cita, dto.Estado, dto.ObservacionCancelacion);
        return Map(_citaRepository.Update(cita));
    }

    public CitaResponseDto Cancelar(int id, CancelarCitaDto dto)
    {
        var cita = ObtenerCita(id);
        AplicarTransicion(cita, EstadoCita.Cancelada, dto.ObservacionCancelacion);
        return Map(_citaRepository.Update(cita));
    }

    public TriajeResponseDto RegistrarTriaje(int citaId, CrearTriajeDto dto)
    {
        var cita = ObtenerCita(citaId);

        if (cita.Estado is not (EstadoCita.Confirmada or EstadoCita.EnTriaje))
        {
            throw new BusinessException("Solo se puede registrar triaje en citas Confirmada o EnTriaje.");
        }

        if (_citaRepository.GetTriajeByCitaId(citaId) is not null)
        {
            throw new ConflictException("La cita ya tiene un triaje registrado.");
        }

        var triaje = new Triaje
        {
            CitaId = citaId,
            Sintomas = dto.Sintomas,
            PresionArterial = dto.PresionArterial,
            FrecuenciaCardiaca = dto.FrecuenciaCardiaca,
            Temperatura = dto.Temperatura,
            PesoKg = dto.PesoKg,
            TallaCm = dto.TallaCm,
            Prioridad = dto.Prioridad
        };

        cita.Estado = EstadoCita.EnTriaje;
        cita.FechaHoraLlegada ??= DateTime.UtcNow;
        _citaRepository.Update(cita);

        return MapTriaje(_citaRepository.AddTriaje(triaje));
    }

    public TriajeResponseDto ObtenerTriaje(int citaId)
    {
        ObtenerCita(citaId);
        var triaje = _citaRepository.GetTriajeByCitaId(citaId)
            ?? throw new NotFoundException($"No existe triaje para la cita {citaId}.");
        return MapTriaje(triaje);
    }

    public AtencionResponseDto RegistrarAtencion(int citaId, CrearAtencionDto dto)
    {
        var cita = ObtenerCita(citaId);

        if (cita.Estado is not (EstadoCita.EnTriaje or EstadoCita.EnAtencion))
        {
            throw new BusinessException("La atención requiere que la cita esté en EnTriaje o EnAtencion.");
        }

        if (_citaRepository.GetAtencionByCitaId(citaId) is not null)
        {
            throw new ConflictException("La cita ya tiene una atención registrada.");
        }

        var atencion = new AtencionMedica
        {
            CitaId = citaId,
            DiagnosticoCIE10 = dto.DiagnosticoCIE10,
            Evolucion = dto.Evolucion,
            Tratamiento = dto.Tratamiento,
            RecetaIndicaciones = dto.RecetaIndicaciones,
            Estado = dto.Estado
        };

        cita.Estado = EstadoCita.EnAtencion;
        _citaRepository.Update(cita);

        return MapAtencion(_citaRepository.AddAtencion(atencion));
    }

    public AtencionResponseDto ActualizarAtencion(int citaId, ActualizarAtencionDto dto)
    {
        ObtenerCita(citaId);
        var atencion = _citaRepository.GetAtencionByCitaId(citaId)
            ?? throw new NotFoundException($"No existe atención para la cita {citaId}.");

        atencion.DiagnosticoCIE10 = dto.DiagnosticoCIE10;
        atencion.Evolucion = dto.Evolucion;
        atencion.Tratamiento = dto.Tratamiento;
        atencion.RecetaIndicaciones = dto.RecetaIndicaciones;
        atencion.Estado = dto.Estado;

        if (dto.Estado == EstadoAtencion.Completada)
        {
            var cita = ObtenerCita(citaId);
            cita.Estado = EstadoCita.Finalizada;
            _citaRepository.Update(cita);
        }

        return MapAtencion(_citaRepository.UpdateAtencion(atencion));
    }

    public AtencionResponseDto ObtenerAtencion(int citaId)
    {
        ObtenerCita(citaId);
        var atencion = _citaRepository.GetAtencionByCitaId(citaId)
            ?? throw new NotFoundException($"No existe atención para la cita {citaId}.");
        return MapAtencion(atencion);
    }

    public PagoResponseDto RegistrarPago(int citaId, CrearPagoDto dto)
    {
        var cita = ObtenerCita(citaId);

        if (cita.Estado == EstadoCita.Cancelada)
        {
            throw new BusinessException("No se puede registrar pago de una cita cancelada.");
        }

        if (_citaRepository.GetPagoByCitaId(citaId) is not null)
        {
            throw new ConflictException("La cita ya tiene un pago registrado.");
        }

        var pago = new PagoTransaccion
        {
            CitaId = citaId,
            MontoTotal = dto.MontoTotal,
            MetodoPago = dto.MetodoPago,
            EstadoPago = dto.EstadoPago
        };

        return MapPago(_citaRepository.AddPago(pago));
    }

    public PagoResponseDto ObtenerPago(int citaId)
    {
        ObtenerCita(citaId);
        var pago = _citaRepository.GetPagoByCitaId(citaId)
            ?? throw new NotFoundException($"No existe pago para la cita {citaId}.");
        return MapPago(pago);
    }

    private CitaMedica ObtenerCita(int id) =>
        _citaRepository.GetById(id)
        ?? throw new NotFoundException($"No se encontró la cita con ID {id}.");

    private void ValidarParticipantes(int pacienteId, int medicoId, int especialidadId)
    {
        var paciente = _pacienteRepository.GetById(pacienteId);
        if (paciente is null || !paciente.Activo)
            throw new BusinessException($"El paciente con ID {pacienteId} no existe o está inactivo.");

        var medico = _medicoRepository.GetById(medicoId);
        if (medico is null || !medico.Activo)
            throw new BusinessException($"El médico con ID {medicoId} no existe o está inactivo.");

        var especialidad = _especialidadRepository.GetById(especialidadId);
        if (especialidad is null || !especialidad.Activo)
            throw new BusinessException($"La especialidad con ID {especialidadId} no existe o está inactiva.");
    }

    private static void AplicarTransicion(CitaMedica cita, EstadoCita nuevoEstado, string? observacionCancelacion)
    {
        if (!TransicionesPermitidas.TryGetValue(cita.Estado, out var permitidos) || !permitidos.Contains(nuevoEstado))
        {
            throw new BusinessException($"No se permite la transición de {cita.Estado} a {nuevoEstado}.");
        }

        if (nuevoEstado == EstadoCita.Cancelada)
        {
            if (string.IsNullOrWhiteSpace(observacionCancelacion))
            {
                throw new BusinessException("La observación de cancelación es obligatoria.");
            }
            cita.ObservacionCancelacion = observacionCancelacion.Trim();
        }

        cita.Estado = nuevoEstado;
    }

    private CitaResponseDto Map(CitaMedica c)
    {
        var paciente = _pacienteRepository.GetById(c.PacienteId);
        var medico = _medicoRepository.GetById(c.MedicoId);
        var especialidad = _especialidadRepository.GetById(c.EspecialidadId);

        return new CitaResponseDto
        {
            Id = c.Id,
            CodigoCita = c.CodigoCita,
            PacienteId = c.PacienteId,
            PacienteNombre = paciente?.NombreCompleto,
            MedicoId = c.MedicoId,
            MedicoNombre = medico?.NombreCompleto,
            EspecialidadId = c.EspecialidadId,
            EspecialidadNombre = especialidad?.Nombre,
            FechaHoraCita = c.FechaHoraCita,
            MotivoConsulta = c.MotivoConsulta,
            Estado = c.Estado,
            FechaHoraLlegada = c.FechaHoraLlegada,
            ObservacionCancelacion = c.ObservacionCancelacion,
            FechaRegistro = c.FechaRegistro
        };
    }

    private static TriajeResponseDto MapTriaje(Triaje t) => new()
    {
        Id = t.Id,
        CitaId = t.CitaId,
        FechaHoraTriaje = t.FechaHoraTriaje,
        Sintomas = t.Sintomas,
        PresionArterial = t.PresionArterial,
        FrecuenciaCardiaca = t.FrecuenciaCardiaca,
        Temperatura = t.Temperatura,
        PesoKg = t.PesoKg,
        TallaCm = t.TallaCm,
        Prioridad = t.Prioridad
    };

    private static AtencionResponseDto MapAtencion(AtencionMedica a) => new()
    {
        Id = a.Id,
        CitaId = a.CitaId,
        FechaHoraInicio = a.FechaHoraInicio,
        DiagnosticoCIE10 = a.DiagnosticoCIE10,
        Evolucion = a.Evolucion,
        Tratamiento = a.Tratamiento,
        RecetaIndicaciones = a.RecetaIndicaciones,
        Estado = a.Estado
    };

    private static PagoResponseDto MapPago(PagoTransaccion p) => new()
    {
        Id = p.Id,
        CitaId = p.CitaId,
        CodigoTransaccion = p.CodigoTransaccion,
        MontoTotal = p.MontoTotal,
        MetodoPago = p.MetodoPago,
        EstadoPago = p.EstadoPago,
        FechaHoraPago = p.FechaHoraPago
    };
}
