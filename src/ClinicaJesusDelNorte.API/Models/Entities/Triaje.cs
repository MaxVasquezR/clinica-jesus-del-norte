using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Registro de triaje de una cita (relación 1:1).
/// </summary>
public class Triaje : EntidadAuditable
{
    public int CitaId { get; set; }
    public DateTime FechaHoraTriaje { get; set; }
    public string? Sintomas { get; set; }
    public string? PresionArterial { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public int? FrecuenciaRespiratoria { get; set; }
    public int? SaturacionOxigeno { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public PrioridadTriaje Prioridad { get; set; } = PrioridadTriaje.Media;
}
