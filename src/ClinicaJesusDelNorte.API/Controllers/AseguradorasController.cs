using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Aseguradoras: lectura para todo el personal; registro y cambios solo Administrador.
/// </summary>
[ApiController]
[Route("api/aseguradoras")]
[Produces("application/json")]
[Authorize]
public class AseguradorasController : ControllerBase
{
    private readonly IAseguradoraService _service;

    public AseguradorasController(IAseguradoraService service)
    {
        _service = service;
    }

    /// <summary>Lista paginada de aseguradoras.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AseguradoraResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<PagedResult<AseguradoraResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<AseguradoraResponseDto>>.Ok(data, "Aseguradoras obtenidas correctamente."));
    }

    /// <summary>Obtiene una aseguradora por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AseguradoraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<AseguradoraResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<AseguradoraResponseDto>.Ok(data));
    }

    /// <summary>Registra una aseguradora. Rol: Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<AseguradoraResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<AseguradoraResponseDto>> Create([FromBody] AseguradoraDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<AseguradoraResponseDto>.Ok(data, "Aseguradora registrada correctamente."));
    }

    /// <summary>Actualiza una aseguradora. Rol: Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<AseguradoraResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<AseguradoraResponseDto>> Update(int id, [FromBody] AseguradoraDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<AseguradoraResponseDto>.Ok(data, "Aseguradora actualizada correctamente."));
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
        return Ok(ApiResponse<object>.Ok(new { id }, "Aseguradora dada de baja correctamente."));
    }
}
