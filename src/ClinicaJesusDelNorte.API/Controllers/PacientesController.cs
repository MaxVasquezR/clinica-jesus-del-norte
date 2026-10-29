using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

[ApiController]
[Route("api/pacientes")]
[Produces("application/json")]
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<PacienteResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<PacienteResponseDto>.Ok(data));
    }

    /// <summary>Registra un nuevo paciente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Create([FromBody] CrearPacienteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<PacienteResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<PacienteResponseDto>.Ok(data, "Paciente registrado correctamente."));
    }

    /// <summary>Actualiza la información demográfica o de seguro del paciente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<PacienteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<PacienteResponseDto>> Update(int id, [FromBody] ActualizarPacienteDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<PacienteResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<PacienteResponseDto>.Ok(data, "Paciente actualizado correctamente."));
    }

    /// <summary>Da de baja lógica al paciente.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Paciente dado de baja correctamente."));
    }
}
