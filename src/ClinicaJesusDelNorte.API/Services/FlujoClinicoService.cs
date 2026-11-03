using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;
using ClinicaJesusDelNorte.API.Seguridad;

namespace ClinicaJesusDelNorte.API.Services;

/// <summary>
/// Triaje (Enfermera) y atención, diagnósticos y receta (solo el médico asignado a la cita).
/// </summary>
public class FlujoClinicoService : IFlujoClinicoService
{
    private readonly IRepositorio<Triaje> _triajes;
    private readonly IRepositorio<AtencionMedica> _atenciones;
    private readonly IRepositorio<AtencionDiagnostico> _diagnosticos;
    private readonly IRepositorio<RecetaDetalle> _recetas;
    private readonly IRepositorio<Cie10> _cie10;
    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IRepositorio<Medico> _medicos;
    private readonly CitaFlujo _flujo;
    private readonly IUsuarioActual _usuarioActual;

    public FlujoClinicoService(
        IRepositorio<Triaje> triajes,
        IRepositorio<AtencionMedica> atenciones,
        IRepositorio<AtencionDiagnostico> diagnosticos,
        IRepositorio<RecetaDetalle> recetas,
        IRepositorio<Cie10> cie10,
        IRepositorio<Usuario> usuarios,
        IRepositorio<Medico> medicos,
        CitaFlujo flujo,
        IUsuarioActual usuarioActual)
    {
        _triajes = triajes;
        _atenciones = atenciones;
        _diagnosticos = diagnosticos;
        _recetas = recetas;
        _cie10 = cie10;
        _usuarios = usuarios;
        _medicos = medicos;
        _flujo = flujo;
        _usuarioActual = usuarioActual;
    }

    public TriajeResponseDto RegistrarTriaje(int citaId, CrearTriajeDto dto)
    {
        var cita = _flujo.Obtener(citaId);

        if (_triajes.Any(t => t.CitaId == citaId))
            throw new ConflictException("La cita ya tiene un triaje registrado.");
        if (cita.Estado != EstadoCita.Confirmada)
            throw new BusinessException($"Solo se registra triaje en citas Confirmada (estado actual: {cita.Estado}).");
        if (cita.FechaHoraLlegada is null)
            throw new BusinessException("Recepción debe registrar primero la llegada del paciente.");

        var triaje = _triajes.Add(new Triaje
        {
            CitaId = citaId,
            FechaHoraTriaje = DateTime.Now,
            Sintomas = Texto.Limpiar(dto.Sintomas),
            PresionArterial = Texto.Limpiar(dto.PresionArterial),
            FrecuenciaCardiaca = dto.FrecuenciaCardiaca,
            FrecuenciaRespiratoria = dto.FrecuenciaRespiratoria,
            SaturacionOxigeno = dto.SaturacionOxigeno,
            Temperatura = dto.Temperatura,
            PesoKg = dto.PesoKg,
            TallaCm = dto.TallaCm,
            Prioridad = dto.Prioridad
        });

        _flujo.CambiarEstado(cita, EstadoCita.EnTriaje, $"Triaje con prioridad {dto.Prioridad}");
        return MapTriaje(triaje);
    }

    public TriajeResponseDto ObtenerTriaje(int citaId)
    {
        _flujo.Obtener(citaId);
        var triaje = _triajes.FirstOrDefault(t => t.CitaId == citaId)
            ?? throw new NotFoundException($"No existe triaje para la cita {citaId}.");
        return MapTriaje(triaje);
    }

    public AtencionResponseDto IniciarAtencion(int citaId, IniciarAtencionDto dto)
    {
        var cita = _flujo.Obtener(citaId);
        ValidarMedicoAsignado(cita);

        if (_atenciones.Any(a => a.CitaId == citaId))
            throw new ConflictException("La cita ya tiene una atención registrada.");
        if (cita.Estado != EstadoCita.EnTriaje)
            throw new BusinessException($"La atención empieza después del triaje (estado actual: {cita.Estado}).");

        var atencion = _atenciones.Add(new AtencionMedica
        {
            CitaId = citaId,
            FechaHoraInicio = DateTime.Now,
            Evolucion = Texto.Limpiar(dto.Evolucion),
            Tratamiento = Texto.Limpiar(dto.Tratamiento),
            Estado = EstadoAtencion.EnCurso
        });

        _flujo.CambiarEstado(cita, EstadoCita.EnAtencion, null);
        return MapAtencion(atencion);
    }

    public AtencionResponseDto ActualizarAtencion(int citaId, ActualizarAtencionDto dto)
    {
        var (cita, atencion) = ObtenerAtencionEnCurso(citaId);
        var evolucion = Texto.Limpiar(dto.Evolucion);

        if (dto.Estado == EstadoAtencion.Completada)
        {
            var diagnosticos = _diagnosticos.Find(d => d.AtencionId == atencion.Id);
            if (diagnosticos.Count == 0)
                throw new BusinessException("Registre al menos un diagnóstico antes de completar la atención.");
            if (diagnosticos.Count(d => d.Principal) != 1)
                throw new BusinessException("La atención debe tener exactamente un diagnóstico principal.");
        }

        if (dto.Estado == EstadoAtencion.Suspendida && evolucion is null)
            throw new BusinessException("Indique en la evolución el motivo de la suspensión.");

        // Se asigna recién después de validar: el repositorio guarda la misma instancia,
        // así que un error a mitad de camino dejaría cambios sin auditoría.
        atencion.Evolucion = evolucion;
        atencion.Tratamiento = Texto.Limpiar(dto.Tratamiento);
        atencion.Estado = dto.Estado;
        if (dto.Estado != EstadoAtencion.EnCurso)
        {
            // La consulta terminó: completada o suspendida, la cita queda Finalizada.
            atencion.FechaHoraFin = DateTime.Now;
            _flujo.CambiarEstado(cita, EstadoCita.Finalizada, dto.Estado == EstadoAtencion.Suspendida ? "Atención suspendida" : null);
        }

        return MapAtencion(_atenciones.Update(atencion));
    }

    public AtencionResponseDto ObtenerAtencion(int citaId)
    {
        _flujo.Obtener(citaId);
        var atencion = _atenciones.FirstOrDefault(a => a.CitaId == citaId)
            ?? throw new NotFoundException($"No existe atención para la cita {citaId}.");
        return MapAtencion(atencion);
    }

    public AtencionDiagnosticoResponseDto AgregarDiagnostico(int citaId, CrearDiagnosticoDto dto)
    {
        var (_, atencion) = ObtenerAtencionEnCurso(citaId);

        var cie10 = _cie10.GetById(dto.Cie10Id);
        if (cie10 is null || !cie10.Activo)
            throw new BusinessException($"El diagnóstico CIE-10 con ID {dto.Cie10Id} no existe o está inactivo.");
        if (_diagnosticos.Any(d => d.AtencionId == atencion.Id && d.Cie10Id == cie10.Id))
            throw new ConflictException($"El diagnóstico {cie10.Codigo} ya está registrado en esta atención.");

        var esPrimero = !_diagnosticos.Any(d => d.AtencionId == atencion.Id);
        var diagnostico = _diagnosticos.Add(new AtencionDiagnostico
        {
            AtencionId = atencion.Id,
            Cie10Id = cie10.Id,
            Principal = esPrimero || dto.Principal,
            Observacion = Texto.Limpiar(dto.Observacion)
        });

        if (diagnostico.Principal)
            QuitarOtrosPrincipales(atencion.Id, diagnostico.Id);

        return MapDiagnostico(diagnostico);
    }

    public AtencionDiagnosticoResponseDto MarcarPrincipal(int citaId, int diagnosticoId)
    {
        var (_, atencion) = ObtenerAtencionEnCurso(citaId);

        var diagnostico = _diagnosticos.GetById(diagnosticoId);
        if (diagnostico is null || diagnostico.AtencionId != atencion.Id)
            throw new NotFoundException($"La atención de la cita {citaId} no tiene el diagnóstico con ID {diagnosticoId}.");

        if (!diagnostico.Principal)
        {
            diagnostico.Principal = true;
            _diagnosticos.Update(diagnostico);
            QuitarOtrosPrincipales(atencion.Id, diagnostico.Id);
        }

        return MapDiagnostico(diagnostico);
    }

    public RecetaDetalleResponseDto AgregarReceta(int citaId, CrearRecetaDetalleDto dto)
    {
        var (_, atencion) = ObtenerAtencionEnCurso(citaId);

        var receta = _recetas.Add(new RecetaDetalle
        {
            AtencionId = atencion.Id,
            Medicamento = dto.Medicamento.Trim(),
            Dosis = dto.Dosis.Trim(),
            Frecuencia = dto.Frecuencia.Trim(),
            DuracionDias = dto.DuracionDias,
            Indicaciones = Texto.Limpiar(dto.Indicaciones)
        });

        return MapReceta(receta);
    }

    private (CitaMedica Cita, AtencionMedica Atencion) ObtenerAtencionEnCurso(int citaId)
    {
        var cita = _flujo.Obtener(citaId);
        ValidarMedicoAsignado(cita);

        var atencion = _atenciones.FirstOrDefault(a => a.CitaId == citaId)
            ?? throw new NotFoundException($"No existe atención para la cita {citaId}. Iníciela primero.");
        if (atencion.Estado != EstadoAtencion.EnCurso)
            throw new BusinessException($"La atención ya está {atencion.Estado} y no admite cambios.");

        return (cita, atencion);
    }

    // El usuario de la sesión debe ser, a través de su Persona, el médico de la cita.
    private void ValidarMedicoAsignado(CitaMedica cita)
    {
        var usuario = _usuarioActual.UsuarioId is int id ? _usuarios.GetById(id) : null;
        var medico = usuario is null ? null : _medicos.FirstOrDefault(m => m.PersonaId == usuario.PersonaId);

        if (medico is null || medico.Id != cita.MedicoId)
            throw new ForbiddenException("Solo el médico asignado a la cita puede registrar su atención.");
    }

    private void QuitarOtrosPrincipales(int atencionId, int principalId)
    {
        foreach (var otro in _diagnosticos.Find(d => d.AtencionId == atencionId && d.Id != principalId && d.Principal))
        {
            otro.Principal = false;
            _diagnosticos.Update(otro);
        }
    }

    private static TriajeResponseDto MapTriaje(Triaje t) => new TriajeResponseDto
    {
        Id = t.Id,
        CitaId = t.CitaId,
        FechaHoraTriaje = t.FechaHoraTriaje,
        Sintomas = t.Sintomas,
        PresionArterial = t.PresionArterial,
        FrecuenciaCardiaca = t.FrecuenciaCardiaca,
        FrecuenciaRespiratoria = t.FrecuenciaRespiratoria,
        SaturacionOxigeno = t.SaturacionOxigeno,
        Temperatura = t.Temperatura,
        PesoKg = t.PesoKg,
        TallaCm = t.TallaCm,
        Prioridad = t.Prioridad
    }.ConAuditoria(t);

    private AtencionResponseDto MapAtencion(AtencionMedica a) => new AtencionResponseDto
    {
        Id = a.Id,
        CitaId = a.CitaId,
        FechaHoraInicio = a.FechaHoraInicio,
        FechaHoraFin = a.FechaHoraFin,
        Evolucion = a.Evolucion,
        Tratamiento = a.Tratamiento,
        Estado = a.Estado,
        Diagnosticos = _diagnosticos.Find(d => d.AtencionId == a.Id).Select(MapDiagnostico).ToList(),
        Receta = _recetas.Find(r => r.AtencionId == a.Id).Select(MapReceta).ToList()
    }.ConAuditoria(a);

    private AtencionDiagnosticoResponseDto MapDiagnostico(AtencionDiagnostico d)
    {
        var cie10 = _cie10.GetById(d.Cie10Id);
        return new AtencionDiagnosticoResponseDto
        {
            Id = d.Id,
            AtencionId = d.AtencionId,
            Cie10Id = d.Cie10Id,
            Codigo = cie10?.Codigo ?? string.Empty,
            Descripcion = cie10?.Descripcion ?? string.Empty,
            Principal = d.Principal,
            Observacion = d.Observacion
        }.ConAuditoria(d);
    }

    private static RecetaDetalleResponseDto MapReceta(RecetaDetalle r) => new RecetaDetalleResponseDto
    {
        Id = r.Id,
        AtencionId = r.AtencionId,
        Medicamento = r.Medicamento,
        Dosis = r.Dosis,
        Frecuencia = r.Frecuencia,
        DuracionDias = r.DuracionDias,
        Indicaciones = r.Indicaciones
    }.ConAuditoria(r);
}
