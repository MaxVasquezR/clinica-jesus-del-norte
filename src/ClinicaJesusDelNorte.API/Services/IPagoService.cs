using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IPagoService
{
    IReadOnlyList<PagoResponseDto> Listar(int citaId);
    PagoResponseDto Registrar(int citaId, CrearPagoDto dto);
    PagoResponseDto CambiarEstado(int citaId, int pagoId, CambiarEstadoPagoDto dto);
}
