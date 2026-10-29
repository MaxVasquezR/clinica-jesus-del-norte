using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Controllers;

[ApiController]
[Route("api/medicos")]
[Produces("application/json")]
public class MedicosController : ControllerBase
{
    private readonly IMedicoService _service;

    public MedicosController(IMedicoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicoResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagedResult<MedicoResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<MedicoResponseDto>>.Ok(data, "Médicos obtenidos correctamente."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<MedicoResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Create([FromBody] CrearMedicoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MedicoResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<MedicoResponseDto>.Ok(data, "Médico registrado correctamente."));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Update(int id, [FromBody] ActualizarMedicoDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<MedicoResponseDto>.Fail(
                "Datos de entrada inválidos.",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
        }

        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data, "Médico actualizado correctamente."));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Médico dado de baja correctamente."));
    }
}
