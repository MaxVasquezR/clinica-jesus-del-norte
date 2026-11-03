namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Paciente de la clínica. CodigoPaciente es su número de historia clínica.
/// </summary>
public class Paciente : EntidadAuditable
{
    public int PersonaId { get; set; }
    public string CodigoPaciente { get; set; } = string.Empty;
    public int? AseguradoraId { get; set; }
    public string? NumeroPoliza { get; set; }
    public string? Alergias { get; set; }
    public bool Activo { get; set; } = true;
}
