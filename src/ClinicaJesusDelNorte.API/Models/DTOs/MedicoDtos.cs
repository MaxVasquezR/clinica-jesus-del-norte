using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearMedicoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public DatosPersonaDto Persona { get; set; } = new();

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(20, MinimumLength = 3, ErrorMessage = ValidationMessages.LongitudRango)]
    public string CodigoColegiatura { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Rne { get; set; }

    [MinLength(1, ErrorMessage = "Debe indicar al menos una especialidad.")]
    public List<int> EspecialidadIds { get; set; } = [];

    public bool Activo { get; set; } = true;
}

public class ActualizarMedicoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    public ActualizarPersonaDto Persona { get; set; } = new();

    [StringLength(30, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Rne { get; set; }

    [MinLength(1, ErrorMessage = "Debe indicar al menos una especialidad.")]
    public List<int> EspecialidadIds { get; set; } = [];

    public bool Activo { get; set; } = true;
}

public class MedicoResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public string CodigoColegiatura { get; set; } = string.Empty;
    public string? Rne { get; set; }
    public bool Activo { get; set; }
    public PersonaResponseDto Persona { get; set; } = new();
    public List<MedicoEspecialidadResponseDto> Especialidades { get; set; } = [];
}

public class MedicoEspecialidadResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int EspecialidadId { get; set; }
    public string EspecialidadNombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class HorarioMedicoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(DiaSemana), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public DiaSemana DiaSemana { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [RegularExpression("^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = ValidationMessages.Hora)]
    public string HoraInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [RegularExpression("^([01][0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = ValidationMessages.Hora)]
    public string HoraFin { get; set; } = string.Empty;

    [Range(5, 240, ErrorMessage = ValidationMessages.Rango)]
    public int DuracionCitaMinutos { get; set; } = 30;

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Consultorio { get; set; }

    public bool Activo { get; set; } = true;
}

public class HorarioMedicoResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int MedicoId { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int DuracionCitaMinutos { get; set; }
    public string? Consultorio { get; set; }
    public bool Activo { get; set; }
}
