namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Médico colegiado; sus especialidades están en MedicoEspecialidad.
/// </summary>
public class Medico : EntidadAuditable
{
    public int PersonaId { get; set; }
    public string CodigoColegiatura { get; set; } = string.Empty;
    public string? Rne { get; set; }
    public bool Activo { get; set; } = true;
}
