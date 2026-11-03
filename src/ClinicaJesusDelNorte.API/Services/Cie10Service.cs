using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class Cie10Service : ICie10Service
{
    private readonly IRepositorio<Cie10> _repository;

    public Cie10Service(IRepositorio<Cie10> repository)
    {
        _repository = repository;
    }

    public PagedResult<Cie10ResponseDto> Listar(int page, int size, string? buscar)
    {
        var texto = Texto.Limpiar(buscar);
        var items = texto is null
            ? _repository.GetAll()
            : _repository.Find(c => c.Codigo.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                    || c.Descripcion.Contains(texto, StringComparison.OrdinalIgnoreCase));
        return Paginador.Crear(items, page, size, Map);
    }

    public Cie10ResponseDto ObtenerPorId(int id)
    {
        return Map(Obtener(id));
    }

    public Cie10ResponseDto Crear(CrearCie10Dto dto)
    {
        var codigo = dto.Codigo.Trim().ToUpperInvariant();
        if (_repository.Any(c => c.Codigo == codigo))
            throw new ConflictException($"El código CIE-10 {codigo} ya está registrado.");

        return Map(_repository.Add(new Cie10 { Codigo = codigo, Descripcion = dto.Descripcion.Trim(), Activo = dto.Activo }));
    }

    public Cie10ResponseDto Actualizar(int id, ActualizarCie10Dto dto)
    {
        var cie10 = Obtener(id);
        cie10.Descripcion = dto.Descripcion.Trim();
        cie10.Activo = dto.Activo;
        return Map(_repository.Update(cie10));
    }

    public void Eliminar(int id)
    {
        var cie10 = Obtener(id);
        cie10.Activo = false;
        _repository.Update(cie10);
    }

    private Cie10 Obtener(int id) =>
        _repository.GetById(id) ?? throw new NotFoundException($"No se encontró el diagnóstico CIE-10 con ID {id}.");

    private static Cie10ResponseDto Map(Cie10 c) => new Cie10ResponseDto
    {
        Id = c.Id,
        Codigo = c.Codigo,
        Descripcion = c.Descripcion,
        Activo = c.Activo
    }.ConAuditoria(c);
}
