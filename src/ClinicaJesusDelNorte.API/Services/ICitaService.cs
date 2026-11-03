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

    TriajeResponseDto RegistrarTriaje(int citaId, CrearTriajeDto dto);
    TriajeResponseDto ObtenerTriaje(int citaId);

    AtencionResponseDto RegistrarAtencion(int citaId, CrearAtencionDto dto);
    AtencionResponseDto ActualizarAtencion(int citaId, ActualizarAtencionDto dto);
    AtencionResponseDto ObtenerAtencion(int citaId);

    PagoResponseDto RegistrarPago(int citaId, CrearPagoDto dto);
    PagoResponseDto ObtenerPago(int citaId);
}
