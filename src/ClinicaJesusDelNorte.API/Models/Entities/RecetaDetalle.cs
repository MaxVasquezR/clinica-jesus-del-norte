namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Medicamento recetado en una atención.
/// </summary>
public class RecetaDetalle : EntidadAuditable
{
    public int AtencionId { get; set; }
    public string Medicamento { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
    public string? Indicaciones { get; set; }
}
