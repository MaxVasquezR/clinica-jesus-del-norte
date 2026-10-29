using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public class EspecialidadRepository : IEspecialidadRepository
{
    private readonly IInMemoryStore _store;

    public EspecialidadRepository(IInMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyList<Especialidad> GetAll()
    {
        lock (_store.SyncRoot)
        {
            return _store.Especialidades.OrderBy(e => e.Id).ToList();
        }
    }

    public Especialidad? GetById(int id)
    {
        lock (_store.SyncRoot)
        {
            return _store.Especialidades.FirstOrDefault(e => e.Id == id);
        }
    }

    public Especialidad? GetByNombre(string nombre)
    {
        lock (_store.SyncRoot)
        {
            return _store.Especialidades.FirstOrDefault(e =>
                e.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Especialidad Add(Especialidad especialidad)
    {
        lock (_store.SyncRoot)
        {
            especialidad.Id = _store.NextEspecialidadId();
            _store.Especialidades.Add(especialidad);
            return especialidad;
        }
    }

    public Especialidad Update(Especialidad especialidad)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Especialidades.FindIndex(e => e.Id == especialidad.Id);
            if (index >= 0)
            {
                _store.Especialidades[index] = especialidad;
            }
            return especialidad;
        }
    }

    public bool SoftDelete(int id)
    {
        lock (_store.SyncRoot)
        {
            var especialidad = _store.Especialidades.FirstOrDefault(e => e.Id == id);
            if (especialidad is null) return false;
            especialidad.Activo = false;
            return true;
        }
    }
}
