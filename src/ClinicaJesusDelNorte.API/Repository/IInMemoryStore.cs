using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

/// <summary>
/// Almacén en memoria que simula la persistencia de una base de datos.
/// </summary>
public interface IInMemoryStore
{
    List<Paciente> Pacientes { get; }
    List<Medico> Medicos { get; }
    List<Especialidad> Especialidades { get; }
    List<CitaMedica> Citas { get; }
    List<Triaje> Triajes { get; }
    List<AtencionMedica> Atenciones { get; }
    List<PagoTransaccion> Pagos { get; }

    object SyncRoot { get; }

    int NextPacienteId();
    int NextMedicoId();
    int NextEspecialidadId();
    int NextCitaId();
    int NextTriajeId();
    int NextAtencionId();
    int NextPagoId();
    int NextCitaSequence();
}
