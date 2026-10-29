using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class EspecialidadService : IEspecialidadService
{
    private readonly IEspecialidadRepository _repository;

    public EspecialidadService(IEspecialidadRepository repository)
    {
        _repository = repository;
    }

    public PagedResult<EspecialidadResponseDto> Listar(int page, int size)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, 100);

        var all = _repository.GetAll();
        var total = all.Count;
        var items = all.Skip(page * size).Take(size).Select(Map).ToList();

        return new PagedResult<EspecialidadResponseDto>
        {
            Items = items,
            Page = page,
            Size = size,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)size)
        };
    }

    public EspecialidadResponseDto ObtenerPorId(int id)
    {
        var especialidad = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró la especialidad con ID {id}.");
        return Map(especialidad);
    }

    public EspecialidadResponseDto Crear(CrearEspecialidadDto dto)
    {
        if (_repository.GetByNombre(dto.Nombre) is not null)
        {
            throw new ConflictException($"Ya existe una especialidad con el nombre '{dto.Nombre}'.");
        }

        var especialidad = new Especialidad
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = dto.Descripcion,
            PrecioBase = dto.PrecioBase,
            Activo = dto.Activo
        };

        return Map(_repository.Add(especialidad));
    }

    public EspecialidadResponseDto Actualizar(int id, ActualizarEspecialidadDto dto)
    {
        var especialidad = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró la especialidad con ID {id}.");

        var otra = _repository.GetByNombre(dto.Nombre);
        if (otra is not null && otra.Id != id)
        {
            throw new ConflictException($"Ya existe una especialidad con el nombre '{dto.Nombre}'.");
        }

        especialidad.Nombre = dto.Nombre.Trim();
        especialidad.Descripcion = dto.Descripcion;
        especialidad.PrecioBase = dto.PrecioBase;
        especialidad.Activo = dto.Activo;

        return Map(_repository.Update(especialidad));
    }

    public void Eliminar(int id)
    {
        if (!_repository.SoftDelete(id))
        {
            throw new NotFoundException($"No se encontró la especialidad con ID {id}.");
        }
    }

    private static EspecialidadResponseDto Map(Especialidad e) => new()
    {
        Id = e.Id,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        PrecioBase = e.PrecioBase,
        Activo = e.Activo
    };
}
