using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class AseguradoraService : IAseguradoraService
{
    private readonly IRepositorio<Aseguradora> _repository;

    public AseguradoraService(IRepositorio<Aseguradora> repository)
    {
        _repository = repository;
    }

    public PagedResult<AseguradoraResponseDto> Listar(int page, int size)
    {
        return Paginador.Crear(_repository.GetAll(), page, size, Map);
    }

    public AseguradoraResponseDto ObtenerPorId(int id)
    {
        return Map(Obtener(id));
    }

    public AseguradoraResponseDto Crear(AseguradoraDto dto)
    {
        ValidarNombreUnico(dto.Nombre, null);
        return Map(_repository.Add(new Aseguradora { Nombre = dto.Nombre.Trim(), Activo = dto.Activo }));
    }

    public AseguradoraResponseDto Actualizar(int id, AseguradoraDto dto)
    {
        var aseguradora = Obtener(id);
        ValidarNombreUnico(dto.Nombre, id);
        aseguradora.Nombre = dto.Nombre.Trim();
        aseguradora.Activo = dto.Activo;
        return Map(_repository.Update(aseguradora));
    }

    public void Eliminar(int id)
    {
        var aseguradora = Obtener(id);
        aseguradora.Activo = false;
        _repository.Update(aseguradora);
    }

    private Aseguradora Obtener(int id) =>
        _repository.GetById(id) ?? throw new NotFoundException($"No se encontró la aseguradora con ID {id}.");

    private void ValidarNombreUnico(string nombre, int? id)
    {
        var limpio = nombre.Trim();
        if (_repository.Any(a => a.Id != id && a.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Ya existe una aseguradora con el nombre '{limpio}'.");
    }

    private static AseguradoraResponseDto Map(Aseguradora a) => new AseguradoraResponseDto
    {
        Id = a.Id,
        Nombre = a.Nombre,
        Activo = a.Activo
    }.ConAuditoria(a);
}
