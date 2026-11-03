using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IPersonaService
{
    Persona? BuscarPorDocumento(TipoDocumento tipoDocumento, string numeroDocumento);
    Persona ObtenerOCrear(DatosPersonaDto dto);
    Persona Actualizar(int personaId, ActualizarPersonaDto dto);
    PersonaResponseDto Map(int personaId);
}
