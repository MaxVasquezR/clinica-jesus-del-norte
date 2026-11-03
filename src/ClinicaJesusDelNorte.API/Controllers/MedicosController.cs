using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Médicos: lectura para todo el personal; registro y cambios solo Administrador.
/// </summary>
[ApiController]
[Route("api/medicos")]
[Produces("application/json")]
[Authorize]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    /// <summary>Lista paginada de médicos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<PagedResult<MedicoResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20,
        [FromQuery] int? especialidadId = null)
    {
        var data = _service.Listar(page, size, especialidadId);
        return Ok(ApiResponse<PagedResult<MedicoResponseDto>>.Ok(data, "Médicos obtenidos correctamente."));
    }

    /// <summary>Obtiene un médico por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<MedicoResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data));
    }

    /// <summary>Registra un médico. Rol: Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Create([FromBody] CrearMedicoDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<MedicoResponseDto>.Ok(data, "Médico registrado correctamente."));
    }

    /// <summary>Actualiza un médico. Rol: Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Update(int id, [FromBody] ActualizarMedicoDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data, "Médico actualizado correctamente."));
    }

    /// <summary>Baja lógica (Activo = false). Rol: Administrador.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Médico dado de baja correctamente."));
    }

    /// <summary>Horario semanal del médico.</summary>
    [HttpGet("{medicoId:int}/horarios")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<HorarioMedicoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<IReadOnlyList<HorarioMedicoResponseDto>>> GetHorarios(int medicoId)
    {
        var data = _service.ListarHorarios(medicoId);
        return Ok(ApiResponse<IReadOnlyList<HorarioMedicoResponseDto>>.Ok(data));
    }

    /// <summary>Agrega un bloque de atención al médico. Rol: Administrador.</summary>
    [HttpPost("{medicoId:int}/horarios")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<HorarioMedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<HorarioMedicoResponseDto>> CrearHorario(int medicoId, [FromBody] HorarioMedicoDto dto)
    {
        var data = _service.CrearHorario(medicoId, dto);
        return CreatedAtAction(nameof(GetHorarios), new { medicoId },
            ApiResponse<HorarioMedicoResponseDto>.Ok(data, "Horario registrado correctamente."));
    }

    /// <summary>Modifica un bloque de atención. Rol: Administrador.</summary>
    [HttpPut("{medicoId:int}/horarios/{horarioId:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<HorarioMedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<HorarioMedicoResponseDto>> ActualizarHorario(int medicoId, int horarioId, [FromBody] HorarioMedicoDto dto)
    {
        var data = _service.ActualizarHorario(medicoId, horarioId, dto);
        return Ok(ApiResponse<HorarioMedicoResponseDto>.Ok(data, "Horario actualizado correctamente."));
    }

    /// <summary>Desactiva un bloque de atención. Rol: Administrador.</summary>
    [HttpDelete("{medicoId:int}/horarios/{horarioId:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> EliminarHorario(int medicoId, int horarioId)
    {
        _service.EliminarHorario(medicoId, horarioId);
        return Ok(ApiResponse<object>.Ok(new { id = horarioId }, "Horario desactivado correctamente."));
    }
}
