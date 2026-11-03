using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Bloque semanal de atención de un médico. Las horas van en formato HH:mm.
/// </summary>
public class HorarioMedico : EntidadAuditable
{
    public int MedicoId { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public int DuracionCitaMinutos { get; set; }
    public string? Consultorio { get; set; }
    public bool Activo { get; set; } = true;
}
