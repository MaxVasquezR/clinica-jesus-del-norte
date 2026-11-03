using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Perfil de acceso: define qué puede hacer un usuario en el sistema.
/// </summary>
public class Rol : EntidadAuditable
{
    public TipoRol Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
