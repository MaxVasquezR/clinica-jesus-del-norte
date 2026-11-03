using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearEspecialidadDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 3, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Descripcion { get; set; }

    [Range(0, 99999.99, ErrorMessage = "El precio base debe ser mayor o igual a 0.")]
    public decimal PrecioBase { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarEspecialidadDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 3, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Descripcion { get; set; }

    [Range(0, 99999.99, ErrorMessage = ValidationMessages.Rango)]
    public decimal PrecioBase { get; set; }

    public bool Activo { get; set; } = true;
}

public class EspecialidadResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioBase { get; set; }
    public bool Activo { get; set; }
}
