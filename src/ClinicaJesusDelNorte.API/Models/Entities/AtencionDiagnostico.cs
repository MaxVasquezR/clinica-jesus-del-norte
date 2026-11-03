namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Diagnóstico CIE-10 registrado en una atención. Cada atención tiene un solo principal.
/// </summary>
public class AtencionDiagnostico : EntidadAuditable
{
    public int AtencionId { get; set; }
    public int Cie10Id { get; set; }
    public bool Principal { get; set; }
    public string? Observacion { get; set; }
}
