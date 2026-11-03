using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Seguridad;

/// <summary>
/// Nombres de rol para [Authorize(Roles = ...)]. Coinciden con <see cref="TipoRol"/>.
/// </summary>
public static class Roles
{
    public const string Administrador = nameof(TipoRol.Administrador);
    public const string Recepcionista = nameof(TipoRol.Recepcionista);
    public const string Enfermera = nameof(TipoRol.Enfermera);
    public const string Medico = nameof(TipoRol.Medico);
    public const string Cajero = nameof(TipoRol.Cajero);

    public const string Clinico = Enfermera + "," + Medico;
    public const string ConsultaDePagos = Administrador + "," + Recepcionista + "," + Cajero;
}
