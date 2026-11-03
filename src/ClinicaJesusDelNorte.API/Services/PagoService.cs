using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class PagoService : IPagoService
{
    private readonly IRepositorio<PagoTransaccion> _pagos;
    private readonly CitaFlujo _flujo;
    private readonly Secuencias _secuencias;

    public PagoService(IRepositorio<PagoTransaccion> pagos, CitaFlujo flujo, Secuencias secuencias)
    {
        _pagos = pagos;
        _flujo = flujo;
        _secuencias = secuencias;
    }

    public IReadOnlyList<PagoResponseDto> Listar(int citaId)
    {
        _flujo.Obtener(citaId);
        return _pagos.Find(p => p.CitaId == citaId).Select(Map).ToList();
    }

    public PagoResponseDto Registrar(int citaId, CrearPagoDto dto)
    {
        var cita = _flujo.Obtener(citaId);

        if (cita.Estado is EstadoCita.Cancelada or EstadoCita.NoAsistio)
            throw new BusinessException($"No se puede registrar el pago de una cita en estado {cita.Estado}.");

        var vigente = _pagos.FirstOrDefault(p => p.CitaId == citaId && p.EstadoPago == EstadoPago.Pagado);
        if (vigente is not null)
            throw new ConflictException($"La cita ya tiene el pago vigente {vigente.CodigoTransaccion}. Anúlelo antes de registrar otro.");

        var ahora = DateTime.Now;
        var esFactura = dto.TipoComprobante == TipoComprobante.Factura;
        var pago = new PagoTransaccion
        {
            CitaId = citaId,
            CodigoTransaccion = $"TXN-{ahora:yyyyMMdd}-{_secuencias.Siguiente("pago"):D5}",
            MontoTotal = dto.MontoTotal,
            MetodoPago = dto.MetodoPago,
            EstadoPago = EstadoPago.Pagado,
            NumeroOperacion = dto.MetodoPago == MetodoPago.Efectivo ? null : Texto.Limpiar(dto.NumeroOperacion),
            TipoComprobante = dto.TipoComprobante,
            NumeroComprobante = esFactura
                ? $"F001-{_secuencias.Siguiente("factura"):D6}"
                : $"B001-{_secuencias.Siguiente("boleta"):D6}",
            RucCliente = esFactura ? dto.RucCliente : null,
            RazonSocialCliente = esFactura ? Texto.Limpiar(dto.RazonSocialCliente) : null,
            FechaHoraPago = ahora
        };

        return Map(_pagos.Add(pago));
    }

    public PagoResponseDto CambiarEstado(int citaId, int pagoId, CambiarEstadoPagoDto dto)
    {
        _flujo.Obtener(citaId);
        var pago = _pagos.GetById(pagoId);
        if (pago is null || pago.CitaId != citaId)
            throw new NotFoundException($"La cita {citaId} no tiene el pago con ID {pagoId}.");

        if (dto.Estado is not (EstadoPago.Anulado or EstadoPago.Reembolsado))
            throw new BusinessException("Un pago registrado solo puede pasar a Anulado o Reembolsado.");
        if (pago.EstadoPago != EstadoPago.Pagado)
            throw new BusinessException($"El pago ya está {pago.EstadoPago}.");

        pago.EstadoPago = dto.Estado;
        pago.MotivoAnulacion = dto.Motivo.Trim();
        return Map(_pagos.Update(pago));
    }

    private static PagoResponseDto Map(PagoTransaccion p) => new PagoResponseDto
    {
        Id = p.Id,
        CitaId = p.CitaId,
        CodigoTransaccion = p.CodigoTransaccion,
        MontoTotal = p.MontoTotal,
        MetodoPago = p.MetodoPago,
        EstadoPago = p.EstadoPago,
        NumeroOperacion = p.NumeroOperacion,
        TipoComprobante = p.TipoComprobante,
        NumeroComprobante = p.NumeroComprobante,
        RucCliente = p.RucCliente,
        RazonSocialCliente = p.RazonSocialCliente,
        MotivoAnulacion = p.MotivoAnulacion,
        FechaHoraPago = p.FechaHoraPago
    }.ConAuditoria(p);
}
