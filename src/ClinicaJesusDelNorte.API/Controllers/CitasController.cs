using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

[ApiController]
[Route("api/citas")]
[Produces("application/json")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _service;

    public CitasController(ICitaService service)
    {
        _service = service;
    }

    /// <summary>Lista citas con filtros y paginación.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CitaResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagedResult<CitaResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20,
        [FromQuery] EstadoCita? estado = null,
        [FromQuery] int? medicoId = null,
        [FromQuery] int? pacienteId = null)
    {
        var data = _service.Listar(page, size, estado, medicoId, pacienteId);
        return Ok(ApiResponse<PagedResult<CitaResponseDto>>.Ok(data, "Citas obtenidas correctamente."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<CitaResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data));
    }

    /// <summary>Reserva una cita vinculando paciente, médico y especialidad.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Create([FromBody] CrearCitaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CitaResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<CitaResponseDto>.Ok(data, "Cita registrada correctamente."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<CitaResponseDto>> Update(int id, [FromBody] ActualizarCitaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CitaResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Cita actualizada correctamente."));
    }

    /// <summary>Cambia el estado de la cita de forma controlada.</summary>
    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> CambiarEstado(int id, [FromBody] CambiarEstadoCitaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CitaResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.CambiarEstado(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Estado de la cita actualizado."));
    }

    /// <summary>Cancela una cita registrando la observación de anulación.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<CitaResponseDto>> Delete(int id, [FromBody] CancelarCitaDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<CitaResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Cancelar(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Cita cancelada correctamente."));
    }

    // --- Rutas jerárquicas ---

    [HttpPost("{citaId:int}/triaje")]
    [ProducesResponseType(typeof(ApiResponse<TriajeResponseDto>), StatusCodes.Status201Created)]
    public ActionResult<ApiResponse<TriajeResponseDto>> CrearTriaje(int citaId, [FromBody] CrearTriajeDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<TriajeResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.RegistrarTriaje(citaId, dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<TriajeResponseDto>.Ok(data, "Triaje registrado correctamente."));
    }

    [HttpGet("{citaId:int}/triaje")]
    [ProducesResponseType(typeof(ApiResponse<TriajeResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<TriajeResponseDto>> GetTriaje(int citaId)
    {
        var data = _service.ObtenerTriaje(citaId);
        return Ok(ApiResponse<TriajeResponseDto>.Ok(data));
    }

    [HttpPost("{citaId:int}/atencion")]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status201Created)]
    public ActionResult<ApiResponse<AtencionResponseDto>> CrearAtencion(int citaId, [FromBody] CrearAtencionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<AtencionResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.RegistrarAtencion(citaId, dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AtencionResponseDto>.Ok(data, "Atención registrada correctamente."));
    }

    [HttpGet("{citaId:int}/atencion")]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<AtencionResponseDto>> GetAtencion(int citaId)
    {
        var data = _service.ObtenerAtencion(citaId);
        return Ok(ApiResponse<AtencionResponseDto>.Ok(data));
    }

    [HttpPut("{citaId:int}/atencion")]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<AtencionResponseDto>> ActualizarAtencion(int citaId, [FromBody] ActualizarAtencionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<AtencionResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.ActualizarAtencion(citaId, dto);
        return Ok(ApiResponse<AtencionResponseDto>.Ok(data, "Atención actualizada correctamente."));
    }

    [HttpPost("{citaId:int}/pagos")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status201Created)]
    public ActionResult<ApiResponse<PagoResponseDto>> CrearPago(int citaId, [FromBody] CrearPagoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<PagoResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.RegistrarPago(citaId, dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<PagoResponseDto>.Ok(data, "Pago registrado correctamente."));
    }

    [HttpGet("{citaId:int}/pagos")]
    [ProducesResponseType(typeof(ApiResponse<PagoResponseDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagoResponseDto>> GetPago(int citaId)
    {
        var data = _service.ObtenerPago(citaId);
        return Ok(ApiResponse<PagoResponseDto>.Ok(data));
    }
}
