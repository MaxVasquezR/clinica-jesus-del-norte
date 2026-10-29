using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Registro de triaje asociado a una cita (relación 1:1).
/// </summary>
public class Triaje
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraTriaje { get; set; } = DateTime.UtcNow;
    public string? Sintomas { get; set; }
    public string? PresionArterial { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public PrioridadTriaje Prioridad { get; set; } = PrioridadTriaje.Media;
}
