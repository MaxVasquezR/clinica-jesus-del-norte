using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearCitaDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un paciente válido.")]
    public int PacienteId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un médico válido.")]
    public int MedicoId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una especialidad válida.")]
    public int EspecialidadId { get; set; }

    [Required]
    public DateTime FechaHoraCita { get; set; }

    [StringLength(500)]
    public string? MotivoConsulta { get; set; }
}

public class ActualizarCitaDto
{
    [StringLength(500)]
    public string? MotivoConsulta { get; set; }

    public DateTime? FechaHoraCita { get; set; }

    public DateTime? FechaHoraLlegada { get; set; }

    [EnumDataType(typeof(EstadoCita))]
    public EstadoCita? Estado { get; set; }

    [StringLength(500)]
    public string? ObservacionCancelacion { get; set; }
}

public class CambiarEstadoCitaDto
{
    [Required]
    [EnumDataType(typeof(EstadoCita))]
    public EstadoCita Estado { get; set; }

    [StringLength(500)]
    public string? ObservacionCancelacion { get; set; }
}

public class CancelarCitaDto
{
    [Required(ErrorMessage = "La observación de cancelación es obligatoria.")]
    [StringLength(500, MinimumLength = 5)]
    public string ObservacionCancelacion { get; set; } = string.Empty;
}

public class CitaResponseDto
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
    public EstadoCita Estado { get; set; }
    public string EstadoNombre => Estado.ToString();
    public DateTime? FechaHoraLlegada { get; set; }
    public string? ObservacionCancelacion { get; set; }
    public DateTime FechaRegistro { get; set; }
}
