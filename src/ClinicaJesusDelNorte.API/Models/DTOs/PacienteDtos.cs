using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearPacienteDto : IValidatableObject
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public DatosPersonaDto Persona { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una aseguradora válida.")]
    public int? AseguradoraId { get; set; }

    [StringLength(40, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? NumeroPoliza { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Alergias { get; set; }

    public bool Activo { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        PacienteReglas.Poliza(AseguradoraId, NumeroPoliza);
}

public class ActualizarPacienteDto : IValidatableObject
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public ActualizarPersonaDto Persona { get; set; } = new();

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una aseguradora válida.")]
    public int? AseguradoraId { get; set; }

    [StringLength(40, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? NumeroPoliza { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Alergias { get; set; }

    public bool Activo { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
        PacienteReglas.Poliza(AseguradoraId, NumeroPoliza);
}

internal static class PacienteReglas
{
    public static IEnumerable<ValidationResult> Poliza(int? aseguradoraId, string? numeroPoliza)
    {
        if (aseguradoraId.HasValue && string.IsNullOrWhiteSpace(numeroPoliza))
        {
            yield return new ValidationResult("El número de póliza es obligatorio si el paciente tiene aseguradora.", [nameof(numeroPoliza)]);
        }
    }
}

public class PacienteResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string CodigoPaciente { get; set; } = string.Empty;
    public int? AseguradoraId { get; set; }
    public string? AseguradoraNombre { get; set; }
    public string? NumeroPoliza { get; set; }
    public string? Alergias { get; set; }
    public bool Activo { get; set; }
    public PersonaResponseDto Persona { get; set; } = new();
}
