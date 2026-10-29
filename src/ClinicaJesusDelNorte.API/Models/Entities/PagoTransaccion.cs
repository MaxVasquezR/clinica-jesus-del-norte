using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Transacción de pago asociada a una cita médica.
/// </summary>
public class PagoTransaccion
{
    public int Id { get; set; }
    public int CitaId { get; set; }
    public string CodigoTransaccion { get; set; } = string.Empty;
    public decimal MontoTotal { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public EstadoPago EstadoPago { get; set; } = EstadoPago.Pendiente;
    public DateTime FechaHoraPago { get; set; } = DateTime.UtcNow;
}
