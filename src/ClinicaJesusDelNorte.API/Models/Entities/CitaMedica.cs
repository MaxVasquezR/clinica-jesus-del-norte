using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Cita médica. PrecioConsulta congela la tarifa vigente al reservar.
/// </summary>
public class CitaMedica : EntidadAuditable
{
    public string CodigoCita { get; set; } = string.Empty;
    public int PacienteId { get; set; }
    public int MedicoId { get; set; }
    public int EspecialidadId { get; set; }
    public DateTime FechaHoraCita { get; set; }
    public string? MotivoConsulta { get; set; }
    public decimal PrecioConsulta { get; set; }
    public EstadoCita Estado { get; set; } = EstadoCita.Solicitada;
    public DateTime? FechaHoraLlegada { get; set; }
    public string? ObservacionCancelacion { get; set; }
}
