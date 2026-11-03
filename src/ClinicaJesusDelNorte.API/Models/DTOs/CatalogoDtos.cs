using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class AseguradoraDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(100, MinimumLength = 2, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}

public class AseguradoraResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class CrearCie10Dto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [RegularExpression(@"^[A-Z][0-9]{2}(\.[0-9A-Z]{1,4})?$", ErrorMessage = "El campo {0} debe ser un código CIE-10 válido, por ejemplo J06.9.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(300, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}

public class ActualizarCie10Dto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(300, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}

public class Cie10ResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
