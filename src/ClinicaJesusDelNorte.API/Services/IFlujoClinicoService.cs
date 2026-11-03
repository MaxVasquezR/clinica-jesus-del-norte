using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IFlujoClinicoService
{
    TriajeResponseDto RegistrarTriaje(int citaId, CrearTriajeDto dto);
    TriajeResponseDto ObtenerTriaje(int citaId);

    AtencionResponseDto IniciarAtencion(int citaId, IniciarAtencionDto dto);
    AtencionResponseDto ActualizarAtencion(int citaId, ActualizarAtencionDto dto);
    AtencionResponseDto ObtenerAtencion(int citaId);

    AtencionDiagnosticoResponseDto AgregarDiagnostico(int citaId, CrearDiagnosticoDto dto);
    AtencionDiagnosticoResponseDto MarcarPrincipal(int citaId, int diagnosticoId);
    RecetaDetalleResponseDto AgregarReceta(int citaId, CrearRecetaDetalleDto dto);
}
