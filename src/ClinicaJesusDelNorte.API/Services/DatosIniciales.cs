using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

/// <summary>
/// Datos de demostración. Se cargan con los mismos servicios que usa la API,
/// así pasan por sus validaciones y quedan auditados con el usuario SISTEMA.
/// </summary>
public class DatosIniciales
{
    /// <summary>Contraseña de todos los usuarios de demostración.</summary>
    public const string ContrasenaDemo = "CitaSync2026";

    private readonly IRepositorio<Rol> _roles;
    private readonly IAseguradoraService _aseguradoras;
    private readonly IEspecialidadService _especialidades;
    private readonly ICie10Service _cie10;
    private readonly IMedicoService _medicos;
    private readonly IPacienteService _pacientes;
    private readonly IUsuarioService _usuarios;
    private readonly ICitaService _citas;

    public DatosIniciales(
        IRepositorio<Rol> roles,
        IAseguradoraService aseguradoras,
        IEspecialidadService especialidades,
        ICie10Service cie10,
        IMedicoService medicos,
        IPacienteService pacientes,
        IUsuarioService usuarios,
        ICitaService citas)
    {
        _roles = roles;
        _aseguradoras = aseguradoras;
        _especialidades = especialidades;
        _cie10 = cie10;
        _medicos = medicos;
        _pacientes = pacientes;
        _usuarios = usuarios;
        _citas = citas;
    }

    public void Cargar()
    {
        if (_roles.Any(_ => true)) return;

        foreach (var (nombre, descripcion) in new[]
        {
            (TipoRol.Administrador, "Gestiona usuarios, médicos, horarios y catálogos"),
            (TipoRol.Recepcionista, "Registra pacientes y reserva, confirma o cancela citas"),
            (TipoRol.Enfermera, "Registra el triaje"),
            (TipoRol.Medico, "Registra la atención, diagnósticos y recetas"),
            (TipoRol.Cajero, "Registra y anula pagos")
        })
        {
            _roles.Add(new Rol { Nombre = nombre, Descripcion = descripcion });
        }

        foreach (var nombre in new[] { "Rímac Seguros", "Pacífico Seguros", "Sanitas Perú" })
            _aseguradoras.Crear(new AseguradoraDto { Nombre = nombre });

        _especialidades.Crear(new CrearEspecialidadDto { Nombre = "Cardiología", Descripcion = "Atención del sistema cardiovascular", PrecioBase = 120m });
        _especialidades.Crear(new CrearEspecialidadDto { Nombre = "Pediatría", Descripcion = "Atención médica infantil", PrecioBase = 90m });
        _especialidades.Crear(new CrearEspecialidadDto { Nombre = "Traumatología", Descripcion = "Lesiones óseas y articulares", PrecioBase = 110m });

        foreach (var (codigo, descripcion) in new[]
        {
            ("I10", "Hipertensión esencial (primaria)"),
            ("R07.4", "Dolor en el pecho, no especificado"),
            ("I20.9", "Angina de pecho, no especificada"),
            ("J06.9", "Infección aguda de las vías respiratorias superiores, no especificada"),
            ("E11.9", "Diabetes mellitus tipo 2 sin complicaciones"),
            ("K29.7", "Gastritis, no especificada"),
            ("M54.5", "Lumbago no especificado"),
            ("R51", "Cefalea")
        })
        {
            _cie10.Crear(new CrearCie10Dto { Codigo = codigo, Descripcion = descripcion });
        }

        var mendoza = CrearMedico("10203040", "Carla", "Mendoza Ruiz", Sexo.Femenino, "999111222", "carla.mendoza@jesusdelnorte.pe", "CMP-45871", "RNE-11223", 1);
        var salazar = CrearMedico("20304050", "Jorge", "Salazar Quispe", Sexo.Masculino, "999333444", "jorge.salazar@jesusdelnorte.pe", "CMP-56210", "RNE-33445", 2);
        var torres = CrearMedico("30405060", "Ana", "Torres Vidal", Sexo.Femenino, "999555666", "ana.torres@jesusdelnorte.pe", "CMP-67890", "RNE-55667", 3);

        foreach (var dia in new[] { DiaSemana.Lunes, DiaSemana.Martes, DiaSemana.Miercoles, DiaSemana.Jueves, DiaSemana.Viernes, DiaSemana.Sabado })
        {
            _medicos.CrearHorario(mendoza.Id, Horario(dia, "08:00", "14:00", 30, "201"));
            _medicos.CrearHorario(salazar.Id, Horario(dia, "08:00", "14:00", 20, "105"));
            if (dia != DiaSemana.Sabado)
                _medicos.CrearHorario(torres.Id, Horario(dia, "14:00", "20:00", 30, "310"));
        }

        var paciente = _pacientes.Crear(new CrearPacienteDto
        {
            Persona = Persona("40000001", "Juan Carlos", "Pérez Rojas", Sexo.Masculino, "900000001", "juan.perez@example.com")
        });
        _pacientes.Crear(new CrearPacienteDto
        {
            Persona = Persona("40000002", "María Elena", "Flores Díaz", Sexo.Femenino, "900000002", "maria.flores@example.com"),
            AseguradoraId = 2,
            NumeroPoliza = "POL-7788",
            Alergias = "Penicilina"
        });

        CrearUsuario(TipoRol.Administrador, "admin", Persona("41234567", "Pedro", "Ramírez Soto", Sexo.Masculino, "987100200", null));
        CrearUsuario(TipoRol.Recepcionista, "recepcion", Persona("42345678", "María", "Torres Vega", Sexo.Femenino, "987100201", null));
        CrearUsuario(TipoRol.Enfermera, "enfermeria", Persona("43456789", "Elena", "Flores Ríos", Sexo.Femenino, "987100202", null));
        CrearUsuario(TipoRol.Medico, "cmendoza", Persona("10203040", "Carla", "Mendoza Ruiz", Sexo.Femenino, null, null));
        CrearUsuario(TipoRol.Medico, "jsalazar", Persona("20304050", "Jorge", "Salazar Quispe", Sexo.Masculino, null, null));
        CrearUsuario(TipoRol.Cajero, "caja", Persona("44567890", "Julio", "Palacios Ñique", Sexo.Masculino, "987100203", null));

        // Cita demo confirmada para el próximo día con atención (lunes a sábado) a las 10:30.
        var fecha = DateTime.Today.AddDays(1);
        if (fecha.DayOfWeek == DayOfWeek.Sunday) fecha = fecha.AddDays(1);

        var cita = _citas.Crear(new CrearCitaDto
        {
            PacienteId = paciente.Id,
            MedicoId = mendoza.Id,
            EspecialidadId = 1,
            FechaHoraCita = fecha.AddHours(10).AddMinutes(30),
            MotivoConsulta = "Control de presión arterial"
        });
        _citas.CambiarEstado(cita.Id, new CambiarEstadoCitaDto { Estado = EstadoCita.Confirmada, Observacion = "Confirmada por teléfono" });
    }

    private MedicoResponseDto CrearMedico(string dni, string nombres, string apellidos, Sexo sexo, string telefono,
        string email, string colegiatura, string rne, int especialidadId) =>
        _medicos.Crear(new CrearMedicoDto
        {
            Persona = Persona(dni, nombres, apellidos, sexo, telefono, email),
            CodigoColegiatura = colegiatura,
            Rne = rne,
            EspecialidadIds = [especialidadId]
        });

    private void CrearUsuario(TipoRol rol, string username, DatosPersonaDto persona) =>
        _usuarios.Crear(new CrearUsuarioDto
        {
            Persona = persona,
            RolId = _roles.FirstOrDefault(r => r.Nombre == rol)!.Id,
            Username = username,
            Contrasena = ContrasenaDemo
        });

    private static DatosPersonaDto Persona(string dni, string nombres, string apellidos, Sexo sexo, string? telefono, string? email) => new()
    {
        TipoDocumento = TipoDocumento.DNI,
        NumeroDocumento = dni,
        Nombres = nombres,
        Apellidos = apellidos,
        Sexo = sexo,
        Telefono = telefono,
        Email = email
    };

    private static HorarioMedicoDto Horario(DiaSemana dia, string inicio, string fin, int duracion, string consultorio) => new()
    {
        DiaSemana = dia,
        HoraInicio = inicio,
        HoraFin = fin,
        DuracionCitaMinutos = duracion,
        Consultorio = consultorio
    };
}
