using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface ICitaService
{
    PagedResult<CitaResponseDto> Listar(int page, int size, EstadoCita? estado, int? medicoId, int? pacienteId);
    CitaResponseDto ObtenerPorId(int id);
    CitaResponseDto Crear(CrearCitaDto dto);
    CitaResponseDto Actualizar(int id, ActualizarCitaDto dto);
    CitaResponseDto CambiarEstado(int id, CambiarEstadoCitaDto dto);
    CitaResponseDto Cancelar(int id, CancelarCitaDto dto);
    CitaResponseDto RegistrarLlegada(int id);
    IReadOnlyList<HistorialCitaResponseDto> Historial(int id);
}
