using System.Collections.Concurrent;

namespace ClinicaJesusDelNorte.API.Repository;

/// <summary>
/// Correlativos de los códigos de negocio (CJN-, PAC-, TXN-, B001-, F001-).
/// </summary>
public class Secuencias
{
    private readonly ConcurrentDictionary<string, int> _valores = new();

    public int Siguiente(string clave) => _valores.AddOrUpdate(clave, 1, (_, actual) => actual + 1);
}
