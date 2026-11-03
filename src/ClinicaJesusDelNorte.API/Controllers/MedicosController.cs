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

    /// <summary>Lista médicos con paginación.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MedicoResponseDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<PagedResult<MedicoResponseDto>>> GetAll(
        [FromQuery] int page = 0,
        [FromQuery] int size = 20)
    {
        var data = _service.Listar(page, size);
        return Ok(ApiResponse<PagedResult<MedicoResponseDto>>.Ok(data, "Médicos obtenidos correctamente."));
    }

    /// <summary>Obtiene el médico por ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<MedicoResponseDto>> GetById(int id)
    {
        var data = _service.ObtenerPorId(id);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data));
    }

    /// <summary>Registra un médico.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Create([FromBody] CrearMedicoDto dto)
    {
        var data = _service.Crear(dto);
        return CreatedAtAction(nameof(GetById), new { id = data.Id },
            ApiResponse<MedicoResponseDto>.Ok(data, "Médico registrado correctamente."));
    }

    /// <summary>Actualiza un médico.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<MedicoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ApiResponse<MedicoResponseDto>> Update(int id, [FromBody] ActualizarMedicoDto dto)
    {
        var data = _service.Actualizar(id, dto);
        return Ok(ApiResponse<MedicoResponseDto>.Ok(data, "Médico actualizado correctamente."));
    }

    /// <summary>Da de baja lógica a un médico.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public ActionResult<ApiResponse<object>> Delete(int id)
    {
        _service.Eliminar(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Médico dado de baja correctamente."));
    }
}
