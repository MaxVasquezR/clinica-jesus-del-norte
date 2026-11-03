using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IMedicoService
{
    PagedResult<MedicoResponseDto> Listar(int page, int size, int? especialidadId);
    MedicoResponseDto ObtenerPorId(int id);
    MedicoResponseDto Crear(CrearMedicoDto dto);
    MedicoResponseDto Actualizar(int id, ActualizarMedicoDto dto);
    void Eliminar(int id);

    IReadOnlyList<HorarioMedicoResponseDto> ListarHorarios(int medicoId);
    HorarioMedicoResponseDto CrearHorario(int medicoId, HorarioMedicoDto dto);
    HorarioMedicoResponseDto ActualizarHorario(int medicoId, int horarioId, HorarioMedicoDto dto);
    void EliminarHorario(int medicoId, int horarioId);
}
