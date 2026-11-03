using System.ComponentModel.DataAnnotations;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

public class CrearTriajeDto
{
    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Sintomas { get; set; }

    [StringLength(20, ErrorMessage = ValidationMessages.LongitudMaxima)]
    [RegularExpression("^[0-9]{2,3}/[0-9]{2,3}$", ErrorMessage = "El campo {0} debe tener el formato 120/80.")]
    public string? PresionArterial { get; set; }

    [Range(30, 250, ErrorMessage = ValidationMessages.Rango)]
    public int? FrecuenciaCardiaca { get; set; }

    [Range(5, 70, ErrorMessage = ValidationMessages.Rango)]
    public int? FrecuenciaRespiratoria { get; set; }

    [Range(0, 100, ErrorMessage = ValidationMessages.Rango)]
    public int? SaturacionOxigeno { get; set; }

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

public class TriajeResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraTriaje { get; set; }
    public string? Sintomas { get; set; }
    public string? PresionArterial { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public int? FrecuenciaRespiratoria { get; set; }
    public int? SaturacionOxigeno { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public PrioridadTriaje Prioridad { get; set; }
}

public class IniciarAtencionDto
{
    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Evolucion { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Tratamiento { get; set; }
}

public class ActualizarAtencionDto
{
    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Evolucion { get; set; }

    [StringLength(1000, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Tratamiento { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(EstadoAtencion), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoAtencion Estado { get; set; } = EstadoAtencion.EnCurso;
}

public class AtencionResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string? Evolucion { get; set; }
    public string? Tratamiento { get; set; }
    public EstadoAtencion Estado { get; set; }
    public List<AtencionDiagnosticoResponseDto> Diagnosticos { get; set; } = [];
    public List<RecetaDetalleResponseDto> Receta { get; set; } = [];
}

public class CrearDiagnosticoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un diagnóstico CIE-10 válido.")]
    public int Cie10Id { get; set; }

    public bool Principal { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Observacion { get; set; }
}

public class AtencionDiagnosticoResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int AtencionId { get; set; }
    public int Cie10Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Principal { get; set; }
    public string? Observacion { get; set; }
}

public class CrearRecetaDetalleDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(150, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string Medicamento { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(50, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string Dosis { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [StringLength(50, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string Frecuencia { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = ValidationMessages.Rango)]
    public int DuracionDias { get; set; }

    [StringLength(500, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? Indicaciones { get; set; }
}

public class RecetaDetalleResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int AtencionId { get; set; }
    public string Medicamento { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
    public string? Indicaciones { get; set; }
}

public class CrearPagoDto : IValidatableObject
{
    [Range(0.01, 99999.99, ErrorMessage = "El monto debe ser mayor a 0.")]
    public decimal MontoTotal { get; set; }

    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(MetodoPago), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public MetodoPago MetodoPago { get; set; }

    [StringLength(40, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? NumeroOperacion { get; set; }

    [EnumDataType(typeof(TipoComprobante), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public TipoComprobante TipoComprobante { get; set; } = TipoComprobante.Boleta;

    [RegularExpression("^(10|15|17|20)[0-9]{9}$", ErrorMessage = "El campo {0} debe ser un RUC de 11 dígitos que empiece en 10, 15, 17 o 20.")]
    public string? RucCliente { get; set; }

    [StringLength(150, ErrorMessage = ValidationMessages.LongitudMaxima)]
    public string? RazonSocialCliente { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MetodoPago != MetodoPago.Efectivo && string.IsNullOrWhiteSpace(NumeroOperacion))
        {
            yield return new ValidationResult("El número de operación es obligatorio salvo en pagos en efectivo.", [nameof(NumeroOperacion)]);
        }

        if (TipoComprobante == TipoComprobante.Factura)
        {
            if (string.IsNullOrWhiteSpace(RucCliente))
                yield return new ValidationResult("El RUC del cliente es obligatorio en una factura.", [nameof(RucCliente)]);
            if (string.IsNullOrWhiteSpace(RazonSocialCliente))
                yield return new ValidationResult("La razón social del cliente es obligatoria en una factura.", [nameof(RazonSocialCliente)]);
        }
    }
}

/// <summary>
/// Anula o reembolsa un pago. Ambos exigen motivo.
/// </summary>
public class CambiarEstadoPagoDto
{
    [Required(ErrorMessage = ValidationMessages.Obligatorio)]
    [EnumDataType(typeof(EstadoPago), ErrorMessage = ValidationMessages.ValorNoPermitido)]
    public EstadoPago Estado { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(300, MinimumLength = 5, ErrorMessage = ValidationMessages.LongitudRango)]
    public string Motivo { get; set; } = string.Empty;
}

public class PagoResponseDto : AuditoriaResponseDto
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public string CodigoTransaccion { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? NumeroOperacion { get; set; }
    public TipoComprobante TipoComprobante { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public string? RucCliente { get; set; }
    public string? RazonSocialCliente { get; set; }
    public string? MotivoAnulacion { get; set; }
    public DateTime FechaHoraPago { get; set; }
}
