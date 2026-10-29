using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Paciente de la Clínica Jesús del Norte. Hereda datos demográficos de <see cref="Persona"/>.
/// </summary>
public class Paciente : Persona
{
    public string CodigoPaciente { get; set; } = string.Empty;
    public TipoSeguro TipoSeguro { get; set; } = TipoSeguro.Particular;
    public string? NumeroPoliza { get; set; }
}
