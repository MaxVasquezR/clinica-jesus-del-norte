using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Atención médica / historia clínica asociada a una cita (relación 1:1).
/// </summary>
public class AtencionMedica
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraInicio { get; set; } = DateTime.UtcNow;
    public string? DiagnosticoCIE10 { get; set; }
    public string? Evolucion { get; set; }
    public string? Tratamiento { get; set; }
    public string? RecetaIndicaciones { get; set; }
    public EstadoAtencion Estado { get; set; } = EstadoAtencion.EnCurso;
}
