namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Médico colegiado. Hereda datos demográficos de <see cref="Persona"/>.
/// </summary>
public class Medico : Persona
{
    public string CodigoColegiatura { get; set; } = string.Empty;
    public string? Rne { get; set; }
    public int EspecialidadId { get; set; }
}
