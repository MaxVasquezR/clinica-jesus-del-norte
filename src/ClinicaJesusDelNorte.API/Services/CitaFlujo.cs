using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

/// <summary>
/// Reglas compartidas del estado de una cita. Todo cambio de estado deja una fila en HistorialCita.
/// </summary>
public class CitaFlujo
{
    private static readonly Dictionary<EstadoCita, HashSet<EstadoCita>> TransicionesPermitidas = new()
    {
        [EstadoCita.Solicitada] = [EstadoCita.Confirmada, EstadoCita.Cancelada],
        [EstadoCita.Confirmada] = [EstadoCita.EnTriaje, EstadoCita.Cancelada, EstadoCita.NoAsistio],
        [EstadoCita.EnTriaje] = [EstadoCita.EnAtencion, EstadoCita.Cancelada],
        [EstadoCita.EnAtencion] = [EstadoCita.Finalizada, EstadoCita.Cancelada],
        [EstadoCita.Finalizada] = [],
        [EstadoCita.Cancelada] = [],
        [EstadoCita.NoAsistio] = []
    };

    private readonly IRepositorio<CitaMedica> _citas;
    private readonly IRepositorio<HistorialCita> _historial;

    public CitaFlujo(IRepositorio<CitaMedica> citas, IRepositorio<HistorialCita> historial)
    {
        _citas = citas;
        _historial = historial;
    }

    public CitaMedica Obtener(int id) =>
        _citas.GetById(id) ?? throw new NotFoundException($"No se encontró la cita con ID {id}.");

    public CitaMedica Registrar(CitaMedica cita)
    {
        _citas.Add(cita);
        _historial.Add(new HistorialCita { CitaId = cita.Id, EstadoNuevo = cita.Estado, Observacion = "Reserva de la cita" });
        return cita;
    }

    public void CambiarEstado(CitaMedica cita, EstadoCita nuevoEstado, string? observacion)
    {
        if (!TransicionesPermitidas.TryGetValue(cita.Estado, out var permitidos) || !permitidos.Contains(nuevoEstado))
            throw new BusinessException($"No se permite la transición de {cita.Estado} a {nuevoEstado}.");

        var texto = Texto.Limpiar(observacion);
        if (nuevoEstado == EstadoCita.Cancelada)
        {
            cita.ObservacionCancelacion = texto ?? throw new BusinessException("La observación de cancelación es obligatoria.");
        }

        var anterior = cita.Estado;
        cita.Estado = nuevoEstado;
        _citas.Update(cita);
        _historial.Add(new HistorialCita { CitaId = cita.Id, EstadoAnterior = anterior, EstadoNuevo = nuevoEstado, Observacion = texto });
    }
}
