using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearPacienteDto
{
    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    [EnumDataType(typeof(TipoDocumento), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 15 caracteres.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    [StringLength(120, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(TipoSeguro), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoSeguro TipoSeguro { get; set; } = TipoSeguro.Particular;

    [StringLength(40, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? NumeroPoliza { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarPacienteDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = ValidationMessages.Correo)]
    [StringLength(120, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Direccion { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(TipoSeguro), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoSeguro TipoSeguro { get; set; }

    [StringLength(40, ErrorMessage = ValidationMessages.LongitudMaxima)]
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
