using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;

namespace ClinicaJesusDelNorte.API.Services;

public interface IMedicoService
{
    PagedResult<MedicoResponseDto> Listar(int page, int size);
    MedicoResponseDto ObtenerPorId(int id);
    MedicoResponseDto Crear(CrearMedicoDto dto);
    MedicoResponseDto Actualizar(int id, ActualizarMedicoDto dto);
    void Eliminar(int id);
}
