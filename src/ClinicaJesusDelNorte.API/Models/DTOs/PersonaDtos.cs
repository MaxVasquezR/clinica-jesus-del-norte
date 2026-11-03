using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

/// <summary>
/// Datos de persona que se pueden corregir. El documento no cambia después del registro.
/// </summary>
public class ActualizarPersonaDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Apellidos { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [EnumDataType(typeof(Sexo), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public Sexo? Sexo { get; set; }

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = ValidationMessages.Telefono)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = ValidationMessages.Correo)]
    [StringLength(120, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Direccion { get; set; }
}

/// <summary>
/// Datos de persona para un registro nuevo. Si el documento ya existe, se reutiliza esa persona.
/// </summary>
public class DatosPersonaDto : ActualizarPersonaDto
{
    [Required(ErrorMessage = "El tipo de documento es obligatorio.")]
    [EnumDataType(typeof(TipoDocumento), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio.")]
    [StringLength(15, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 15 caracteres.")]
    public string NumeroDocumento { get; set; } = string.Empty;
}

public class PersonaResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public Sexo? Sexo { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
}
