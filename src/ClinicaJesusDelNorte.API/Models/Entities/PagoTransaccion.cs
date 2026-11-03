using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Transacción de pago. Una cita acepta varias, pero solo un pago Pagado vigente.
/// </summary>
public class PagoTransaccion : EntidadAuditable
{
    public int CitaId { get; set; }
    public string CodigoTransaccion { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public EstadoPago EstadoPago { get; set; } = EstadoPago.Pagado;
    public string? NumeroOperacion { get; set; }
    public TipoComprobante TipoComprobante { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public string? RucCliente { get; set; }
    public string? RazonSocialCliente { get; set; }
    public string? MotivoAnulacion { get; set; }
    public DateTime FechaHoraPago { get; set; }
}
