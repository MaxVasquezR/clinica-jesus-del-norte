using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearMedicoDto
{
    [Required]
    [EnumDataType(typeof(TipoDocumento))]
    public TipoDocumento TipoDocumento { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 5)]
    public string NumeroDocumento { get; set; } = string.Empty;

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
    [StringLength(20, MinimumLength = 3)]
    public string CodigoColegiatura { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Rne { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una especialidad válida.")]
    public int EspecialidadId { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarMedicoDto
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

    [StringLength(30)]
    public string? Rne { get; set; }

    [Required]
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
