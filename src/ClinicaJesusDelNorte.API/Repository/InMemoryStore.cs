using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Repository;

/// <summary>
/// Simulación de persistencia local con listas en memoria (thread-safe).
/// </summary>
public class InMemoryStore : IInMemoryStore
{
    private int _pacienteSeq;
    private int _medicoSeq;
    private int _especialidadSeq;
    private int _citaSeq;
    private int _triajeSeq;
    private int _atencionSeq;
    private int _pagoSeq;
    private int _citaCodigoSeq;

    public List<Paciente> Pacientes { get; } = [];
    public List<Medico> Medicos { get; } = [];
    public List<Especialidad> Especialidades { get; } = [];
    public List<CitaMedica> Citas { get; } = [];
    public List<Triaje> Triajes { get; } = [];
    public List<AtencionMedica> Atenciones { get; } = [];
    public List<PagoTransaccion> Pagos { get; } = [];

    public object SyncRoot { get; } = new();

    public InMemoryStore()
    {
        Seed();
    }

    public int NextPacienteId() => Interlocked.Increment(ref _pacienteSeq);
    public int NextMedicoId() => Interlocked.Increment(ref _medicoSeq);
    public int NextEspecialidadId() => Interlocked.Increment(ref _especialidadSeq);
    public int NextCitaId() => Interlocked.Increment(ref _citaSeq);
    public int NextTriajeId() => Interlocked.Increment(ref _triajeSeq);
    public int NextAtencionId() => Interlocked.Increment(ref _atencionSeq);
    public int NextPagoId() => Interlocked.Increment(ref _pagoSeq);
    public int NextCitaSequence() => Interlocked.Increment(ref _citaCodigoSeq);

    private void Seed()
    {
        Especialidades.AddRange(
        [
            new Especialidad { Id = 1, Nombre = "Cardiología", Descripcion = "Atención del sistema cardiovascular", PrecioBase = 120m, Activo = true },
            new Especialidad { Id = 2, Nombre = "Pediatría", Descripcion = "Atención médica infantil", PrecioBase = 90m, Activo = true },
            new Especialidad { Id = 3, Nombre = "Traumatología", Descripcion = "Lesiones óseas y articulares", PrecioBase = 110m, Activo = true }
        ]);
        _especialidadSeq = 3;

        Medicos.AddRange(
        [
            new Medico
            {
                Id = 1,
                TipoDocumento = TipoDocumento.DNI,
                NumeroDocumento = "10203040",
                Nombres = "Carla",
                Apellidos = "Mendoza Ruiz",
                Email = "carla.mendoza@jesusdelnorte.pe",
                Telefono = "999111222",
                CodigoColegiatura = "CMP-45871",
                Rne = "RNE-11223",
                EspecialidadId = 1,
                Activo = true
            },
            new Medico
            {
                Id = 2,
                TipoDocumento = TipoDocumento.DNI,
                NumeroDocumento = "20304050",
                Nombres = "Jorge",
                Apellidos = "Salazar Quispe",
                Email = "jorge.salazar@jesusdelnorte.pe",
                Telefono = "999333444",
                CodigoColegiatura = "CMP-56210",
                Rne = "RNE-33445",
                EspecialidadId = 2,
                Activo = true
            },
            new Medico
            {
                Id = 3,
                TipoDocumento = TipoDocumento.DNI,
                NumeroDocumento = "30405060",
                Nombres = "Ana",
                Apellidos = "Torres Vidal",
                Email = "ana.torres@jesusdelnorte.pe",
                Telefono = "999555666",
                CodigoColegiatura = "CMP-67890",
                Rne = "RNE-55667",
                EspecialidadId = 3,
                Activo = true
            }
        ]);
        _medicoSeq = 3;

        Pacientes.AddRange(
        [
            new Paciente
            {
                Id = 1,
                CodigoPaciente = "PAC-2026-0001",
                TipoDocumento = TipoDocumento.DNI,
                NumeroDocumento = "48129532",
                Nombres = "Max Ayrton",
                Apellidos = "Vásquez Ruiz",
                Email = "max.vasquez@email.com",
                Telefono = "987654321",
                TipoSeguro = TipoSeguro.Particular,
                Activo = true
            },
            new Paciente
            {
                Id = 2,
                CodigoPaciente = "PAC-2026-0002",
                TipoDocumento = TipoDocumento.DNI,
                NumeroDocumento = "55667788",
                Nombres = "Luz Gabriela",
                Apellidos = "Cochachin Camacllanqui",
                Email = "luz.cochachin@email.com",
                Telefono = "912345678",
                TipoSeguro = TipoSeguro.Pacifico,
                NumeroPoliza = "POL-7788",
                Activo = true
            }
        ]);
        _pacienteSeq = 2;

        Citas.Add(new CitaMedica
        {
            Id = 1,
            CodigoCita = "CJN-2026-0001",
            PacienteId = 1,
            MedicoId = 1,
            EspecialidadId = 1,
            FechaHoraCita = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(30),
            MotivoConsulta = "Control de presión arterial",
            Estado = EstadoCita.Confirmada
        });
        _citaSeq = 1;
        _citaCodigoSeq = 1;
    }
}
