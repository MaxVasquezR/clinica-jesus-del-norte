using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public class MedicoRepository : IMedicoRepository
{
    private readonly IInMemoryStore _store;

    public MedicoRepository(IInMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyList<Medico> GetAll()
    {
        lock (_store.SyncRoot)
        {
            return _store.Medicos.OrderBy(m => m.Id).ToList();
        }
    }

    public Medico? GetById(int id)
    {
        lock (_store.SyncRoot)
        {
            return _store.Medicos.FirstOrDefault(m => m.Id == id);
        }
    }

    public Medico? GetByDocumento(string numeroDocumento)
    {
        lock (_store.SyncRoot)
        {
            return _store.Medicos.FirstOrDefault(m =>
                m.NumeroDocumento.Equals(numeroDocumento, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Medico? GetByColegiatura(string codigoColegiatura)
    {
        lock (_store.SyncRoot)
        {
            return _store.Medicos.FirstOrDefault(m =>
                m.CodigoColegiatura.Equals(codigoColegiatura, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Medico Add(Medico medico)
    {
        lock (_store.SyncRoot)
        {
            medico.Id = _store.NextMedicoId();
            medico.FechaRegistro = DateTime.UtcNow;
            _store.Medicos.Add(medico);
            return medico;
        }
    }

    public Medico Update(Medico medico)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Medicos.FindIndex(m => m.Id == medico.Id);
            if (index >= 0)
            {
                _store.Medicos[index] = medico;
            }
            return medico;
        }
    }

    public bool SoftDelete(int id)
    {
        lock (_store.SyncRoot)
        {
            var medico = _store.Medicos.FirstOrDefault(m => m.Id == id);
            if (medico is null) return false;
            medico.Activo = false;
            return true;
        }
    }
}
