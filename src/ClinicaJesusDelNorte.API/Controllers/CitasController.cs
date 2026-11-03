using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Agenda de citas. Reservar, reprogramar, confirmar, marcar llegada y cancelar: Recepcionista.
/// </summary>
[ApiController]
[Route("api/citas")]
[Produces("application/json")]
[Authorize]
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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
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

    /// <summary>Obtiene una cita por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<CitaResponseDto>> GetById(int id)
    {
        return Ok(ApiResponse<CitaResponseDto>.Ok(_service.ObtenerPorId(id)));
    }

    /// <summary>Historial de estados: qué cambió, cuándo y quién lo hizo.</summary>
    [HttpGet("{id:int}/historial")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HistorialCitaResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<IReadOnlyList<HistorialCitaResponseDto>>> GetHistorial(int id)
    {
        return Ok(ApiResponse<IReadOnlyList<HistorialCitaResponseDto>>.Ok(_service.Historial(id)));
    }

    /// <summary>Reserva una cita (estado Solicitada) dentro del horario del médico.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Create([FromBody] CrearCitaDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<CitaResponseDto>.Ok(data, "Cita registrada correctamente."));
    }

    /// <summary>Reprograma la cita o corrige el motivo, antes de la llegada del paciente.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Update(int id, [FromBody] ActualizarCitaDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Cita actualizada correctamente."));
    }

    /// <summary>Confirma, marca NoAsistio o cancela. Los demás estados los asigna el flujo clínico.</summary>
    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> CambiarEstado(int id, [FromBody] CambiarEstadoCitaDto dto)
    {
        var data = _service.CambiarEstado(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Estado de la cita actualizado."));
    }

    /// <summary>Registra la llegada del paciente; la cita pasa a la cola de triaje.</summary>
    [HttpPatch("{id:int}/llegada")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> RegistrarLlegada(int id)
    {
        var data = _service.RegistrarLlegada(id);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Llegada registrada correctamente."));
    }

    /// <summary>Cancela una cita registrando la observación de anulación.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<CitaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<CitaResponseDto>> Delete(int id, [FromBody] CancelarCitaDto dto)
    {
        var data = _service.Cancelar(id, dto);
        return Ok(ApiResponse<CitaResponseDto>.Ok(data, "Cita cancelada correctamente."));
    }
}
