using System.ComponentModel.DataAnnotations;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearEspecialidadDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    [Range(0, 99999.99, ErrorMessage = "El precio base debe ser mayor o igual a 0.")]
    public decimal PrecioBase { get; set; }

    public bool Activo { get; set; } = true;
}

public class ActualizarEspecialidadDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Descripcion { get; set; }

    [Range(0, 99999.99)]
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
