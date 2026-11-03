using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Pacientes: lectura para todo el personal; registro y cambios solo Recepcionista.
/// </summary>
[ApiController]
[Route("api/pacientes")]
[Produces("application/json")]
[Authorize]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _service;

    public PacientesController(IPacienteService service)
    {
        _service = service;
    }

    /// <summary>Lista paginada de pacientes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PacienteResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<PagedResult<PacienteResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<PacienteResponseDto>>.Ok(data, "Pacientes obtenidos correctamente."));
    }

    /// <summary>Obtiene un paciente por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<PacienteResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<PacienteResponseDto>.Ok(data));
    }

    /// <summary>Registra un paciente. Rol: Recepcionista.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Create([FromBody] CrearPacienteDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<PacienteResponseDto>.Ok(data, "Paciente registrado correctamente."));
    }

    /// <summary>Actualiza un paciente. Rol: Recepcionista.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Update(int id, [FromBody] ActualizarPacienteDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<PacienteResponseDto>.Ok(data, "Paciente actualizado correctamente."));
    }

    /// <summary>Baja lógica (Activo = false). Rol: Recepcionista.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Recepcionista)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Paciente dado de baja correctamente."));
    }
}
