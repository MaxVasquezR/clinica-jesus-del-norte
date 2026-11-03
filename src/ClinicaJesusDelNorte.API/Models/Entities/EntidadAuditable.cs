namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Clase base de todas las entidades: identificador y los cuatro campos de auditoría.
/// Los llena el repositorio; ningún servicio los asigna a mano.
/// </summary>
public abstract class EntidadAuditable
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime? FechaActualizacion { get; set; }
    public string? UsuarioActualizacion { get; set; }
}
