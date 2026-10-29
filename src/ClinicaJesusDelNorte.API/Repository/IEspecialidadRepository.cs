using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public interface IEspecialidadRepository
{
    IReadOnlyList<Especialidad> GetAll();
    Especialidad? GetById(int id);
    Especialidad? GetByNombre(string nombre);
    Especialidad Add(Especialidad especialidad);
    Especialidad Update(Especialidad especialidad);
    bool SoftDelete(int id);
}
