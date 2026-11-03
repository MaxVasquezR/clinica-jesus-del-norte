using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;

namespace ClinicaJesusDelNorte.API.Services;

public interface IEspecialidadService
{
    PagedResult<EspecialidadResponseDto> Listar(int page, int size);
    EspecialidadResponseDto ObtenerPorId(int id);
    EspecialidadResponseDto Crear(CrearEspecialidadDto dto);
    EspecialidadResponseDto Actualizar(int id, ActualizarEspecialidadDto dto);
    void Eliminar(int id);
}
