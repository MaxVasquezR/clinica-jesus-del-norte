using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Entities;

/// <summary>
/// Cada cambio de estado de una cita. Cuándo y quién: FechaCreacion y UsuarioCreacion.
/// </summary>
public class HistorialCita : EntidadAuditable
{
    public int CitaId { get; set; }
    public EstadoCita? EstadoAnterior { get; set; }
    public EstadoCita EstadoNuevo { get; set; }
    public string? Observacion { get; set; }
}
