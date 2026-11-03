using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Models.DTOs;

/// <summary>
/// Campos de auditoría que devuelven todas las respuestas.
/// </summary>
public abstract class AuditoriaResponseDto
{
    public DateTime FechaCreacion { get; set; }
    public string UsuarioCreacion { get; set; } = string.Empty;
    public DateTime? FechaActualizacion { get; set; }
    public string? UsuarioActualizacion { get; set; }
}

public static class AuditoriaMapper
{
    public static T ConAuditoria<T>(this T dto, EntidadAuditable entidad) where T : AuditoriaResponseDto
    {
        dto.FechaCreacion = entidad.FechaCreacion;
        dto.UsuarioCreacion = entidad.UsuarioCreacion;
        dto.FechaActualizacion = entidad.FechaActualizacion;
        dto.UsuarioActualizacion = entidad.UsuarioActualizacion;
        return dto;
    }
}
