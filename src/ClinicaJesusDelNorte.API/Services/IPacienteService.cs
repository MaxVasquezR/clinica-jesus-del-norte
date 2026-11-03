using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;

namespace ClinicaJesusDelNorte.API.Services;

public interface IPacienteService
{
    PagedResult<PacienteResponseDto> Listar(int page, int size);
    PacienteResponseDto ObtenerPorId(int id);
    PacienteResponseDto Crear(CrearPacienteDto dto);
    PacienteResponseDto Actualizar(int id, ActualizarPacienteDto dto);
    void Eliminar(int id);
}
