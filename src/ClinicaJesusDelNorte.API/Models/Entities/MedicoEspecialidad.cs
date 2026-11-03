namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Especialidad que atiende un médico (relación N:M con auditoría).
/// </summary>
public class MedicoEspecialidad : EntidadAuditable
{
    public int MedicoId { get; set; }
    public int EspecialidadId { get; set; }
    public bool Activo { get; set; } = true;
}
