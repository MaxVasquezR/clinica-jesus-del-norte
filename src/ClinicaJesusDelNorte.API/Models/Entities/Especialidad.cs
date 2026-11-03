namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Especialidad médica con tarifa base de consulta.
/// </summary>
public class Especialidad : EntidadAuditable
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioBase { get; set; }
    public bool Activo { get; set; } = true;
}
