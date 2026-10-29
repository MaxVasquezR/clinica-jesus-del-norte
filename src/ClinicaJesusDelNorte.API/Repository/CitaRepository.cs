using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Repository;

public class CitaRepository : ICitaRepository
{
    private readonly IInMemoryStore _store;
    private static readonly TimeSpan DuracionConsulta = TimeSpan.FromMinutes(30);

    public CitaRepository(IInMemoryStore store)
    {
        _store = store;
    }

    public IReadOnlyList<CitaMedica> GetAll()
    {
        lock (_store.SyncRoot)
        {
            return _store.Citas
                .OrderByDescending(c => c.FechaHoraCita)
                .ThenBy(c => c.Estado)
                .ToList();
        }
    }

    public CitaMedica? GetById(int id)
    {
        lock (_store.SyncRoot)
        {
            return _store.Citas.FirstOrDefault(c => c.Id == id);
        }
    }

    public CitaMedica Add(CitaMedica cita)
    {
        lock (_store.SyncRoot)
        {
            cita.Id = _store.NextCitaId();
            cita.CodigoCita = GenerateCodigoCitaUnsafe();
            cita.FechaRegistro = DateTime.UtcNow;
            _store.Citas.Add(cita);
            return cita;
        }
    }

    public CitaMedica Update(CitaMedica cita)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Citas.FindIndex(c => c.Id == cita.Id);
            if (index >= 0)
            {
                _store.Citas[index] = cita;
            }
            return cita;
        }
    }

    public bool ExistsOverlap(int medicoId, DateTime fechaHora, int? excludeCitaId = null)
    {
        lock (_store.SyncRoot)
        {
            var inicio = fechaHora;
            var fin = fechaHora.Add(DuracionConsulta);

            return _store.Citas.Any(c =>
                c.MedicoId == medicoId &&
                c.Estado != EstadoCita.Cancelada &&
                c.Estado != EstadoCita.NoAsistio &&
                (!excludeCitaId.HasValue || c.Id != excludeCitaId.Value) &&
                inicio < c.FechaHoraCita.Add(DuracionConsulta) &&
                fin > c.FechaHoraCita);
        }
    }

    public string GenerateCodigoCita()
    {
        lock (_store.SyncRoot)
        {
            return GenerateCodigoCitaUnsafe();
        }
    }

    private string GenerateCodigoCitaUnsafe()
    {
        var seq = _store.NextCitaSequence();
        return $"CJN-{DateTime.UtcNow:yyyy}-{seq:D4}";
    }

    public Triaje? GetTriajeByCitaId(int citaId)
    {
        lock (_store.SyncRoot)
        {
            return _store.Triajes.FirstOrDefault(t => t.CitaId == citaId);
        }
    }

    public Triaje AddTriaje(Triaje triaje)
    {
        lock (_store.SyncRoot)
        {
            triaje.Id = _store.NextTriajeId();
            triaje.FechaHoraTriaje = DateTime.UtcNow;
            _store.Triajes.Add(triaje);
            return triaje;
        }
    }

    public AtencionMedica? GetAtencionByCitaId(int citaId)
    {
        lock (_store.SyncRoot)
        {
            return _store.Atenciones.FirstOrDefault(a => a.CitaId == citaId);
        }
    }

    public AtencionMedica AddAtencion(AtencionMedica atencion)
    {
        lock (_store.SyncRoot)
        {
            atencion.Id = _store.NextAtencionId();
            atencion.FechaHoraInicio = DateTime.UtcNow;
            _store.Atenciones.Add(atencion);
            return atencion;
        }
    }

    public AtencionMedica UpdateAtencion(AtencionMedica atencion)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Atenciones.FindIndex(a => a.Id == atencion.Id);
            if (index >= 0)
            {
                _store.Atenciones[index] = atencion;
            }
            return atencion;
        }
    }

    public PagoTransaccion? GetPagoByCitaId(int citaId)
    {
        lock (_store.SyncRoot)
        {
            return _store.Pagos.FirstOrDefault(p => p.CitaId == citaId);
        }
    }

    public PagoTransaccion AddPago(PagoTransaccion pago)
    {
        lock (_store.SyncRoot)
        {
            pago.Id = _store.NextPagoId();
            pago.CodigoTransaccion = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{pago.Id:D5}";
            pago.FechaHoraPago = DateTime.UtcNow;
            _store.Pagos.Add(pago);
            return pago;
        }
    }
}
