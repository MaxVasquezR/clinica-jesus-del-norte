using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Pagos de una cita. Registrar, anular o reembolsar: Cajero.
/// </summary>
[ApiController]
[Route("api/citas/{citaId:int}/pagos")]
[Produces("application/json")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly IPagoService _service;

    public PagosController(IPagoService service)
    {
        _service = service;
    }

    /// <summary>Pagos de la cita, incluidos los anulados.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.ConsultaDePagos)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PagoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<IReadOnlyList<PagoResponseDto>>> GetAll(int citaId)
    {
        return Ok(ApiResponse<IReadOnlyList<PagoResponseDto>>.Ok(_service.Listar(citaId)));
    }

    /// <summary>Registra un pago con su comprobante. Solo un pago Pagado vigente por cita.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Cajero)]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<PagoResponseDto>> Create(int citaId, [FromBody] CrearPagoDto dto)
    {
        var data = _service.Registrar(citaId, dto);
        return CreatedAtAction(nameof(GetAll), new { citaId },
            ApiResponse<PagoResponseDto>.Ok(data, "Pago registrado correctamente."));
    }

    /// <summary>Anula o reembolsa un pago, con motivo.</summary>
    [HttpPatch("{pagoId:int}/estado")]
    [Authorize(Roles = Roles.Cajero)]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<PagoResponseDto>> CambiarEstado(int citaId, int pagoId, [FromBody] CambiarEstadoPagoDto dto)
    {
        var data = _service.CambiarEstado(citaId, pagoId, dto);
        return Ok(ApiResponse<PagoResponseDto>.Ok(data, "Estado del pago actualizado."));
    }
}
