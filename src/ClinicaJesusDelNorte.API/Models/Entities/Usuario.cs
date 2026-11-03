namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Cuenta con la que el personal inicia sesión. La contraseña solo se guarda como hash.
/// </summary>
public class Usuario : EntidadAuditable
{
    public int PersonaId { get; set; }
    public int RolId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string ContrasenaHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHasta { get; set; }
    public DateTime? UltimoAcceso { get; set; }
}
