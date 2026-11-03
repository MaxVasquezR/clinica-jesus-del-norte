using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IEspecialidadService
{
    PagedResult<EspecialidadResponseDto> Listar(int page, int size);
    EspecialidadResponseDto ObtenerPorId(int id);
    EspecialidadResponseDto Crear(CrearEspecialidadDto dto);
    EspecialidadResponseDto Actualizar(int id, ActualizarEspecialidadDto dto);
    void Eliminar(int id);
}

public interface IAseguradoraService
{
    PagedResult<AseguradoraResponseDto> Listar(int page, int size);
    AseguradoraResponseDto ObtenerPorId(int id);
    AseguradoraResponseDto Crear(AseguradoraDto dto);
    AseguradoraResponseDto Actualizar(int id, AseguradoraDto dto);
    void Eliminar(int id);
}

public interface ICie10Service
{
    PagedResult<Cie10ResponseDto> Listar(int page, int size, string? buscar);
    Cie10ResponseDto ObtenerPorId(int id);
    Cie10ResponseDto Crear(CrearCie10Dto dto);
    Cie10ResponseDto Actualizar(int id, ActualizarCie10Dto dto);
    void Eliminar(int id);
}
