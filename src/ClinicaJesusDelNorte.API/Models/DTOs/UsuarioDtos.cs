using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public string Contrasena { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string TipoToken { get; set; } = "Bearer";
    public DateTime Expira { get; set; }
    public UsuarioResponseDto Usuario { get; set; } = new();
}

public class CrearUsuarioDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public DatosPersonaDto Persona { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un rol válido.")]
    public int RolId { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [RegularExpression("^[a-z0-9._]{4,50}$", ErrorMessage = "El campo {0} debe tener de 4 a 50 caracteres: minúsculas, números, punto o guion bajo.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(64, MinimumLength = 8, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Contrasena { get; set; } = string.Empty;
}

public class ActualizarUsuarioDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un rol válido.")]
    public int RolId { get; set; }

    public bool Activo { get; set; } = true;
}

public class CambiarContrasenaDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(64, MinimumLength = 8, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Contrasena { get; set; } = string.Empty;
}

public class UsuarioResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int RolId { get; set; }
    public TipoRol Rol { get; set; }
    public bool Activo { get; set; }
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHasta { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public PersonaResponseDto Persona { get; set; } = new();
}

public class RolResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public TipoRol Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}
