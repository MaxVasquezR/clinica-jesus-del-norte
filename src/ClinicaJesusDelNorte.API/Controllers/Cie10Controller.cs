using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Diagnósticos cie-10: lectura para todo el personal; registro y cambios solo Administrador.
/// </summary>
[ApiController]
[Route("api/cie10")]
[Produces("application/json")]
[Authorize]
public class Cie10Controller : ControllerBase
{
    private readonly ICie10Service _service;

    public Cie10Controller(ICie10Service service)
    {
        _service = service;
    }

    /// <summary>Lista paginada de diagnósticos CIE-10.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<Cie10ResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<PagedResult<Cie10ResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20,
        [FromQuery] string? buscar = null)
    {
        var data = _service.Listar(page, size, buscar);
        return Ok(ApiResponse<PagedResult<Cie10ResponseDto>>.Ok(data, "Diagnósticos cie-10 obtenidos correctamente."));
    }

    /// <summary>Obtiene un diagnóstico CIE-10 por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<Cie10ResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<Cie10ResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<Cie10ResponseDto>.Ok(data));
    }

    /// <summary>Registra un diagnóstico CIE-10. Rol: Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<Cie10ResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<Cie10ResponseDto>> Create([FromBody] CrearCie10Dto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<Cie10ResponseDto>.Ok(data, "Diagnóstico cie-10 registrado correctamente."));
    }

    /// <summary>Actualiza un diagnóstico CIE-10. Rol: Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<Cie10ResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<Cie10ResponseDto>> Update(int id, [FromBody] ActualizarCie10Dto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<Cie10ResponseDto>.Ok(data, "Diagnóstico cie-10 actualizado correctamente."));
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
        return Ok(ApiResponse<object>.Ok(new { id }, "Diagnóstico cie-10 dado de baja correctamente."));
    }
}
