using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public interface ICitaRepository
{
    IReadOnlyList<CitaMedica> GetAll();
    CitaMedica? GetById(int id);
    CitaMedica Add(CitaMedica cita);
    CitaMedica Update(CitaMedica cita);
    bool ExistsOverlap(int medicoId, DateTime fechaHora, int? excludeCitaId = null);
    string GenerateCodigoCita();

    Triaje? GetTriajeByCitaId(int citaId);
    Triaje AddTriaje(Triaje triaje);

    AtencionMedica? GetAtencionByCitaId(int citaId);
    AtencionMedica AddAtencion(AtencionMedica atencion);
    AtencionMedica UpdateAtencion(AtencionMedica atencion);

    PagoTransaccion? GetPagoByCitaId(int citaId);
    PagoTransaccion AddPago(PagoTransaccion pago);
}
