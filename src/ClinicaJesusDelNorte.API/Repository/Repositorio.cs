using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Seguridad;

namespace ClinicaJesusDelNorte.API.Repository;

/// <summary>
/// Simulación de una tabla con una lista en memoria (thread-safe).
/// Es el único lugar que asigna IDs y campos de auditoría: así ninguna tabla queda sin ellos.
/// </summary>
public class Repositorio<T> : IRepositorio<T> where T : EntidadAuditable
{
    private readonly List<T> _items = [];
    private readonly object _syncRoot = new();
    private readonly IUsuarioActual _usuarioActual;
    private int _secuencia;

    public Repositorio(IUsuarioActual usuarioActual)
    {
        _usuarioActual = usuarioActual;
    }

    public IReadOnlyList<T> GetAll()
    {
        lock (_syncRoot)
        {
            return _items.OrderBy(e => e.Id).ToList();
        }
    }

    public T? GetById(int id)
    {
        lock (_syncRoot)
        {
            return _items.FirstOrDefault(e => e.Id == id);
        }
    }

    public IReadOnlyList<T> Find(Func<T, bool> filtro)
    {
        lock (_syncRoot)
        {
            return _items.Where(filtro).OrderBy(e => e.Id).ToList();
        }
    }

    public T? FirstOrDefault(Func<T, bool> filtro)
    {
        lock (_syncRoot)
        {
            return _items.FirstOrDefault(filtro);
        }
    }

    public bool Any(Func<T, bool> filtro)
    {
        lock (_syncRoot)
        {
            return _items.Any(filtro);
        }
    }

    public T Add(T entidad)
    {
        lock (_syncRoot)
        {
            entidad.Id = ++_secuencia;
            entidad.FechaCreacion = DateTime.UtcNow;
            entidad.UsuarioCreacion = _usuarioActual.Username;
            entidad.FechaActualizacion = null;
            entidad.UsuarioActualizacion = null;
            _items.Add(entidad);
            return entidad;
        }
    }

    public T Update(T entidad)
    {
        lock (_syncRoot)
        {
            var index = _items.FindIndex(e => e.Id == entidad.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"{typeof(T).Name} con ID {entidad.Id} no existe.");
            }

            entidad.FechaActualizacion = DateTime.UtcNow;
            entidad.UsuarioActualizacion = _usuarioActual.Username;
            _items[index] = entidad;
            return entidad;
        }
    }
}
