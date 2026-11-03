using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

/// <summary>
/// Persistencia en memoria de una entidad. Add y Update llenan la auditoría.
/// </summary>
public interface IRepositorio<T> where T : EntidadAuditable
{
    IReadOnlyList<T> GetAll();
    T? GetById(int id);
    IReadOnlyList<T> Find(Func<T, bool> filtro);
    T? FirstOrDefault(Func<T, bool> filtro);
    bool Any(Func<T, bool> filtro);
    T Add(T entidad);
    T Update(T entidad);
}
