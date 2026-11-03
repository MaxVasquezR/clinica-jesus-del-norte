using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Inicio de sesión del personal. Devuelve el token JWT que se envía como "Authorization: Bearer {token}".
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    /// <summary>Inicia sesión. Tres intentos fallidos bloquean el usuario 15 minutos.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var data = _service.Login(dto);
        return Ok(ApiResponse<LoginResponseDto>.Ok(data, "Sesión iniciada correctamente."));
    }

    /// <summary>Datos del usuario de la sesión.</summary>
    [HttpGet("perfil")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public ActionResult<ApiResponse<UsuarioResponseDto>> Perfil()
    {
        return Ok(ApiResponse<UsuarioResponseDto>.Ok(_service.Perfil()));
    }
}
