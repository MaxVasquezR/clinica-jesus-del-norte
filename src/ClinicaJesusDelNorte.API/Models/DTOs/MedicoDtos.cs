using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearMedicoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(TipoDocumento), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoDocumento TipoDocumento { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(15, MinimumLength = 5, ErrorMessage = ValidationMessages.LongitudRango)]
    public string NumeroDocumento { get; set; } = string.Empty;

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
    [StringLength(20, MinimumLength = 3, ErrorMessage = ValidationMessages.LongitudRango)]
    public string CodigoColegiatura { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Rne { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una especialidad válida.")]
    public int EspecialidadId { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarMedicoDto
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

    [StringLength(30, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Rne { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [Range(1, int.MaxValue)]
    public int EspecialidadId { get; set; }

    public bool Activo { get; set; } = true;
}

public class MedicoResponseDto
{
    public int Id { get; set; }
    public string CodigoColegiatura { get; set; } = string.Empty;
    public string? Rne { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public int EspecialidadId { get; set; }
    public string? EspecialidadNombre { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
