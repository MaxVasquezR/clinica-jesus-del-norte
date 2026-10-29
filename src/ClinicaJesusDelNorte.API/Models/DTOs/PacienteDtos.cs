using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearPacienteDto
{
    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    [EnumDataType(typeof(TipoDocumento))]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 15 caracteres.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, MinimumLength = 2)]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }

    [Required]
    [EnumDataType(typeof(TipoSeguro))]
    public TipoSeguro TipoSeguro { get; set; } = TipoSeguro.Particular;

    [StringLength(40)]
    public string? NumeroPoliza { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarPacienteDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress]
    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }

    [Required]
    [EnumDataType(typeof(TipoSeguro))]
    public TipoSeguro TipoSeguro { get; set; }

    [StringLength(40)]
    public string? NumeroPoliza { get; set; }

    public bool Activo { get; set; } = true;
}

public class PacienteResponseDto
{
    public int Id { get; set; }
    public string CodigoPaciente { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public TipoSeguro TipoSeguro { get; set; }
    public string? NumeroPoliza { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
