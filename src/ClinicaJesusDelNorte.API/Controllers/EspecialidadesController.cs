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

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EspecialidadResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagedResult<EspecialidadResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<EspecialidadResponseDto>>.Ok(data, "Especialidades obtenidas correctamente."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<EspecialidadResponseDto>.Ok(data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> Create([FromBody] CrearEspecialidadDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<EspecialidadResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<EspecialidadResponseDto>.Ok(data, "Especialidad registrada correctamente."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<EspecialidadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<EspecialidadResponseDto>> Update(int id, [FromBody] ActualizarEspecialidadDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<EspecialidadResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<EspecialidadResponseDto>.Ok(data, "Especialidad actualizada correctamente."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Especialidad dada de baja correctamente."));
    }
}
