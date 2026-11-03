using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

[ApiController]
[Route("api/especialidades")]
[Produces("application/json")]
public class EspecialidadesController : ControllerBase
{
    private readonly IEspecialidadService _service;

    public EspecialidadesController(IEspecialidadService service)
    {
        _service = service;
    }

    /// <summary>Lista especialidades con paginación.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EspecialidadResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagedResult<EspecialidadResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<EspecialidadResponseDto>>.Ok(data, "Especialidades obtenidas correctamente."));
    }

    /// <summary>Obtiene una especialidad por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<EspecialidadResponseDto>.Ok(data));
    }

    /// <summary>Registra una especialidad.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> Create([FromBody] CrearEspecialidadDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<EspecialidadResponseDto>.Ok(data, "Especialidad registrada correctamente."));
    }

    /// <summary>Actualiza una especialidad.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> Update(int id, [FromBody] ActualizarEspecialidadDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<EspecialidadResponseDto>.Ok(data, "Especialidad actualizada correctamente."));
    }

    /// <summary>Da de baja lógica a una especialidad.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Especialidad dada de baja correctamente."));
    }
}
