using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearTriajeDto
{
    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Sintomas { get; set; }

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? PresionArterial { get; set; }

    [Range(30, 250, ErrorMessage = ValidationMessages.Rango)]
    public int? FrecuenciaCardiaca { get; set; }

    [Range(30, 45, ErrorMessage = ValidationMessages.Rango)]
    public decimal? Temperatura { get; set; }

    [Range(1, 300, ErrorMessage = ValidationMessages.Rango)]
    public decimal? PesoKg { get; set; }

    [Range(30, 250, ErrorMessage = ValidationMessages.Rango)]
    public decimal? TallaCm { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(PrioridadTriaje), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public PrioridadTriaje Prioridad { get; set; } = PrioridadTriaje.Media;
}

public class TriajeResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraTriaje { get; set; }
    public string? Sintomas { get; set; }
    public string? PresionArterial { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public PrioridadTriaje Prioridad { get; set; }
}

public class CrearAtencionDto
{
    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? DiagnosticoCIE10 { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Evolucion { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Tratamiento { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? RecetaIndicaciones { get; set; }

    [EnumDataType(typeof(EstadoAtencion), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoAtencion Estado { get; set; } = EstadoAtencion.EnCurso;
}

public class ActualizarAtencionDto
{
    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? DiagnosticoCIE10 { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Evolucion { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Tratamiento { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? RecetaIndicaciones { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(EstadoAtencion), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoAtencion Estado { get; set; }
}

public class AtencionResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public string? DiagnosticoCIE10 { get; set; }
    public string? Evolucion { get; set; }
    public string? Tratamiento { get; set; }
    public string? RecetaIndicaciones { get; set; }
    public EstadoAtencion Estado { get; set; }
}

public class CrearPagoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [Range(0.01, 99999.99, ErrorMessage = "El monto debe ser mayor a 0.")]
    public decimal MontoTotal { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(MetodoPago), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public MetodoPago MetodoPago { get; set; }

    [EnumDataType(typeof(EstadoPago), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoPago EstadoPago { get; set; } = EstadoPago.Pagado;
}

public class PagoResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public string CodigoTransaccion { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public DateTime FechaHoraPago { get; set; }
}
