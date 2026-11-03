using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class EspecialidadService : IEspecialidadService
{
    private readonly IRepositorio<Especialidad> _repository;

    public EspecialidadService(IRepositorio<Especialidad> repository)
    {
        _repository = repository;
    }

    public PagedResult<EspecialidadResponseDto> Listar(int page, int size)
    {
        return Paginador.Crear(_repository.GetAll(), page, size, Map);
    }

    public EspecialidadResponseDto ObtenerPorId(int id)
    {
        return Map(Obtener(id));
    }

    public EspecialidadResponseDto Crear(CrearEspecialidadDto dto)
    {
        ValidarNombreUnico(dto.Nombre, null);

        var especialidad = new Especialidad
        {
            Nombre = dto.Nombre.Trim(),
            Descripcion = Texto.Limpiar(dto.Descripcion),
            PrecioBase = dto.PrecioBase,
            Activo = dto.Activo
        };

        return Map(_repository.Add(especialidad));
    }

    public EspecialidadResponseDto Actualizar(int id, ActualizarEspecialidadDto dto)
    {
        var especialidad = Obtener(id);
        ValidarNombreUnico(dto.Nombre, id);

        // Las citas ya reservadas conservan su PrecioConsulta aunque cambie la tarifa.
        especialidad.Nombre = dto.Nombre.Trim();
        especialidad.Descripcion = Texto.Limpiar(dto.Descripcion);
        especialidad.PrecioBase = dto.PrecioBase;
        especialidad.Activo = dto.Activo;

        return Map(_repository.Update(especialidad));
    }

    public void Eliminar(int id)
    {
        var especialidad = Obtener(id);
        especialidad.Activo = false;
        _repository.Update(especialidad);
    }

    private Especialidad Obtener(int id) =>
        _repository.GetById(id) ?? throw new NotFoundException($"No se encontró la especialidad con ID {id}.");

    private void ValidarNombreUnico(string nombre, int? id)
    {
        var limpio = nombre.Trim();
        if (_repository.Any(e => e.Id != id && e.Nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Ya existe una especialidad con el nombre '{limpio}'.");
    }

    private static EspecialidadResponseDto Map(Especialidad e) => new EspecialidadResponseDto
    {
        Id = e.Id,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        PrecioBase = e.PrecioBase,
        Activo = e.Activo
    }.ConAuditoria(e);
}
