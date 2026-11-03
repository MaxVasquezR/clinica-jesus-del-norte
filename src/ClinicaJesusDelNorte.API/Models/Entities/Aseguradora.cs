namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Compañía de seguros. Un paciente sin aseguradora es particular.
/// </summary>
public class Aseguradora : EntidadAuditable
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
