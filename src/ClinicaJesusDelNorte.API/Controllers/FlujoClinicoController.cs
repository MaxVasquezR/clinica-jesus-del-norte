using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Seguridad;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

/// <summary>
/// Triaje (Enfermera) y atención médica con diagnósticos y receta (Medico asignado a la cita).
/// </summary>
[ApiController]
[Route("api/citas/{citaId:int}")]
[Produces("application/json")]
[Authorize]
public class FlujoClinicoController : ControllerBase
{
    private readonly IFlujoClinicoService _service;

    public FlujoClinicoController(IFlujoClinicoService service)
    {
        _service = service;
    }

    /// <summary>Registra el triaje; la cita pasa a EnTriaje.</summary>
    [HttpPost("triaje")]
    [Authorize(Roles = Roles.Enfermera)]
    [ProducesResponseType(typeof(ApiResponse<TriajeResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<TriajeResponseDto>> CrearTriaje(int citaId, [FromBody] CrearTriajeDto dto)
    {
        var data = _service.RegistrarTriaje(citaId, dto);
        return CreatedAtAction(nameof(GetTriaje), new { citaId },
            ApiResponse<TriajeResponseDto>.Ok(data, "Triaje registrado correctamente."));
    }

    /// <summary>Obtiene el triaje de una cita.</summary>
    [HttpGet("triaje")]
    [ProducesResponseType(typeof(ApiResponse<TriajeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<TriajeResponseDto>> GetTriaje(int citaId)
    {
        return Ok(ApiResponse<TriajeResponseDto>.Ok(_service.ObtenerTriaje(citaId)));
    }

    /// <summary>Inicia la atención; la cita pasa a EnAtencion.</summary>
    [HttpPost("atencion")]
    [Authorize(Roles = Roles.Medico)]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<AtencionResponseDto>> IniciarAtencion(int citaId, [FromBody] IniciarAtencionDto dto)
    {
        var data = _service.IniciarAtencion(citaId, dto);
        return CreatedAtAction(nameof(GetAtencion), new { citaId },
            ApiResponse<AtencionResponseDto>.Ok(data, "Atención iniciada correctamente."));
    }

    /// <summary>Obtiene la atención con sus diagnósticos y receta.</summary>
    [HttpGet("atencion")]
    [Authorize(Roles = Roles.Clinico)]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<AtencionResponseDto>> GetAtencion(int citaId)
    {
        return Ok(ApiResponse<AtencionResponseDto>.Ok(_service.ObtenerAtencion(citaId)));
    }

    /// <summary>Guarda la evolución y el tratamiento; con Estado = Completada la cita queda Finalizada.</summary>
    [HttpPut("atencion")]
    [Authorize(Roles = Roles.Medico)]
    [ProducesResponseType(typeof(ApiResponse<AtencionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<AtencionResponseDto>> ActualizarAtencion(int citaId, [FromBody] ActualizarAtencionDto dto)
    {
        var data = _service.ActualizarAtencion(citaId, dto);
        return Ok(ApiResponse<AtencionResponseDto>.Ok(data, "Atención actualizada correctamente."));
    }

    /// <summary>Agrega un diagnóstico CIE-10. El primero queda como principal.</summary>
    [HttpPost("atencion/diagnosticos")]
    [Authorize(Roles = Roles.Medico)]
    [ProducesResponseType(typeof(ApiResponse<AtencionDiagnosticoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<AtencionDiagnosticoResponseDto>> AgregarDiagnostico(int citaId, [FromBody] CrearDiagnosticoDto dto)
    {
        var data = _service.AgregarDiagnostico(citaId, dto);
        return CreatedAtAction(nameof(GetAtencion), new { citaId },
            ApiResponse<AtencionDiagnosticoResponseDto>.Ok(data, "Diagnóstico registrado correctamente."));
    }

    /// <summary>Marca un diagnóstico como principal y desmarca el anterior.</summary>
    [HttpPatch("atencion/diagnosticos/{diagnosticoId:int}/principal")]
    [Authorize(Roles = Roles.Medico)]
    [ProducesResponseType(typeof(ApiResponse<AtencionDiagnosticoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<AtencionDiagnosticoResponseDto>> MarcarPrincipal(int citaId, int diagnosticoId)
    {
        var data = _service.MarcarPrincipal(citaId, diagnosticoId);
        return Ok(ApiResponse<AtencionDiagnosticoResponseDto>.Ok(data, "Diagnóstico principal actualizado."));
    }

    /// <summary>Agrega un medicamento a la receta.</summary>
    [HttpPost("atencion/receta")]
    [Authorize(Roles = Roles.Medico)]
    [ProducesResponseType(typeof(ApiResponse<RecetaDetalleResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<RecetaDetalleResponseDto>> AgregarReceta(int citaId, [FromBody] CrearRecetaDetalleDto dto)
    {
        var data = _service.AgregarReceta(citaId, dto);
        return CreatedAtAction(nameof(GetAtencion), new { citaId },
            ApiResponse<RecetaDetalleResponseDto>.Ok(data, "Medicamento agregado a la receta."));
    }
}
