using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public interface IMedicoRepository
{
    IReadOnlyList<Medico> GetAll();
    Medico? GetById(int id);
    Medico? GetByDocumento(string numeroDocumento);
    Medico? GetByColegiatura(string codigoColegiatura);
    Medico Add(Medico medico);
    Medico Update(Medico medico);
    bool SoftDelete(int id);
}
