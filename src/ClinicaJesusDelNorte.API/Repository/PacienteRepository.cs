using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public class PacienteRepository : IPacienteRepository
{
    private readonly IInMemoryStore _store;

    public PacienteRepository(IInMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyList<Paciente> GetAll()
    {
        lock (_store.SyncRoot)
        {
            return _store.Pacientes.OrderBy(p => p.Id).ToList();
        }
    }

    public Paciente? GetById(int id)
    {
        lock (_store.SyncRoot)
        {
            return _store.Pacientes.FirstOrDefault(p => p.Id == id);
        }
    }

    public Paciente? GetByDocumento(string numeroDocumento)
    {
        lock (_store.SyncRoot)
        {
            return _store.Pacientes.FirstOrDefault(p =>
                p.NumeroDocumento.Equals(numeroDocumento, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Paciente Add(Paciente paciente)
    {
        lock (_store.SyncRoot)
        {
            paciente.Id = _store.NextPacienteId();
            paciente.CodigoPaciente = $"PAC-{DateTime.UtcNow:yyyy}-{paciente.Id:D4}";
            paciente.FechaRegistro = DateTime.UtcNow;
            _store.Pacientes.Add(paciente);
            return paciente;
        }
    }

    public Paciente Update(Paciente paciente)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Pacientes.FindIndex(p => p.Id == paciente.Id);
            if (index >= 0)
            {
                _store.Pacientes[index] = paciente;
            }
            return paciente;
        }
    }

    public bool SoftDelete(int id)
    {
        lock (_store.SyncRoot)
        {
            var paciente = _store.Pacientes.FirstOrDefault(p => p.Id == id);
            if (paciente is null) return false;
            paciente.Activo = false;
            return true;
        }
    }

    public int Count()
    {
        lock (_store.SyncRoot)
        {
            return _store.Pacientes.Count;
        }
    }
}
