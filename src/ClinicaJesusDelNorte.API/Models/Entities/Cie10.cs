namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Catálogo de diagnósticos de la clasificación CIE-10.
/// </summary>
public class Cie10 : EntidadAuditable
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
