using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Cuentas del personal y sus roles. Solo Administrador.
/// </summary>
[ApiController]
[Produces("application/json")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    /// <summary>Roles disponibles: quién puede usar el sistema.</summary>
    [HttpGet("api/roles")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RolResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public ActionResult<ApiResponse<IReadOnlyList<RolResponseDto>>> GetRoles()
    {
        return Ok(ApiResponse<IReadOnlyList<RolResponseDto>>.Ok(_service.ListarRoles()));
    }

    /// <summary>Lista paginada de usuarios.</summary>
    [HttpGet("api/usuarios")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UsuarioResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public ActionResult<ApiResponse<PagedResult<UsuarioResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<UsuarioResponseDto>>.Ok(data, "Usuarios obtenidos correctamente."));
    }

    /// <summary>Obtiene un usuario por ID.</summary>
    [HttpGet("api/usuarios/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> GetById(int id)
    {
        return Ok(ApiResponse<UsuarioResponseDto>.Ok(_service.ObtenerPorId(id)));
    }

    /// <summary>Crea un usuario. Si la persona ya existe por documento, se reutiliza.</summary>
    [HttpPost("api/usuarios")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> Create([FromBody] CrearUsuarioDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<UsuarioResponseDto>.Ok(data, "Usuario creado correctamente."));
    }

    /// <summary>Cambia el rol o activa/desactiva un usuario.</summary>
    [HttpPut("api/usuarios/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> Update(int id, [FromBody] ActualizarUsuarioDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<UsuarioResponseDto>.Ok(data, "Usuario actualizado correctamente."));
    }

    /// <summary>Quita el bloqueo por intentos fallidos.</summary>
    [HttpPatch("api/usuarios/{id:int}/desbloquear")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> Desbloquear(int id)
    {
        var data = _service.Desbloquear(id);
        return Ok(ApiResponse<UsuarioResponseDto>.Ok(data, "Usuario desbloqueado correctamente."));
    }

    /// <summary>Asigna una contraseña nueva (se guarda solo su hash).</summary>
    [HttpPatch("api/usuarios/{id:int}/contrasena")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> CambiarContrasena(int id, [FromBody] CambiarContrasenaDto dto)
    {
        var data = _service.CambiarContrasena(id, dto);
        return Ok(ApiResponse<UsuarioResponseDto>.Ok(data, "Contraseña actualizada correctamente."));
    }
}
