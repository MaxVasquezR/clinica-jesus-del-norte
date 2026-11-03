using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Atención médica de una cita (relación 1:1). La historia clínica es el conjunto de atenciones del paciente.
/// </summary>
public class AtencionMedica : EntidadAuditable
{
    public int CitaId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string? Evolucion { get; set; }
    public string? Tratamiento { get; set; }
    public EstadoAtencion Estado { get; set; } = EstadoAtencion.EnCurso;
}
