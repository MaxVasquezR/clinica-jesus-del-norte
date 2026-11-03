namespace ClinicaJesusDelNorte.API.Models.Common;

/// <summary>
/// Pagina colecciones en memoria con los mismos límites en todos los servicios.
/// </summary>
public static class Paginador
{
    private const int TamanioMaximo = 100;

    public static PagedResult<TDestino> Crear<TOrigen, TDestino>(
        IReadOnlyCollection<TOrigen> origen,
        int page,
        int size,
        Func<TOrigen, TDestino> map)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, TamanioMaximo);
        var omitir = (int)Math.Min((long)page * size, int.MaxValue);

        return new PagedResult<TDestino>
        {
            Items = origen.Skip(omitir).Take(size).Select(map).ToList(),
            Page = page,
            Size = size,
            TotalItems = origen.Count,
            TotalPages = (int)Math.Ceiling(origen.Count / (double)size)
        };
    }
}
