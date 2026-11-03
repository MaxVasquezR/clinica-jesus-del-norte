using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearCitaDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un paciente válido.")]
    public int PacienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un médico válido.")]
    public int MedicoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una especialidad válida.")]
    public int EspecialidadId { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public DateTime? FechaHoraCita { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? MotivoConsulta { get; set; }
}

/// <summary>
/// Reprogramación o corrección del motivo. El estado cambia por PATCH /estado o por el flujo clínico.
/// </summary>
public class ActualizarCitaDto
{
    public DateTime? FechaHoraCita { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? MotivoConsulta { get; set; }
}

public class CambiarEstadoCitaDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(EstadoCita), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoCita Estado { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Observacion { get; set; }
}

public class CancelarCitaDto
{
    [Required(ErrorMessage = "La observación de cancelación es obligatoria.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = ValidationMessages.LongitudRango)]
    public string ObservacionCancelacion { get; set; } = string.Empty;
}

public class CitaResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string CodigoCita { get; set; } = string.Empty;
    public int PacienteId { get; set; }
    public string? PacienteNombre { get; set; }
    public int MedicoId { get; set; }
    public string? MedicoNombre { get; set; }
    public int EspecialidadId { get; set; }
    public string? EspecialidadNombre { get; set; }
    public DateTime FechaHoraCita { get; set; }
    public string? MotivoConsulta { get; set; }
    public decimal PrecioConsulta { get; set; }
    public EstadoCita Estado { get; set; }
    public DateTime? FechaHoraLlegada { get; set; }
    public string? ObservacionCancelacion { get; set; }
}

public class HistorialCitaResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public EstadoCita? EstadoAnterior { get; set; }
    public EstadoCita EstadoNuevo { get; set; }
    public string? Observacion { get; set; }
}
