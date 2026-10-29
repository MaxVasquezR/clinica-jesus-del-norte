using ClinicaJesusDelNorte.API.Models.Entities;

namespace ClinicaJesusDelNorte.API.Repository;

public interface IPacienteRepository
{
    IReadOnlyList<Paciente> GetAll();
    Paciente? GetById(int id);
    Paciente? GetByDocumento(string numeroDocumento);
    Paciente Add(Paciente paciente);
    Paciente Update(Paciente paciente);
    bool SoftDelete(int id);
    int Count();
}
