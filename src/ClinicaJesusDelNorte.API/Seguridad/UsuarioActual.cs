using System.Security.Claims;

namespace ClinicaJesusDelNorte.API.Seguridad;

/// <summary>
/// Usuario de la sesión, leído del token JWT. Sin sesión (arranque, login) es SISTEMA.
/// </summary>
public interface IUsuarioActual
{
    string Username { get; }
    int? UsuarioId { get; }
}

public class UsuarioActual : IUsuarioActual
{
    public const string Sistema = "SISTEMA";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActual(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal =>
        _httpContextAccessor.HttpContext?.User is { Identity.IsAuthenticated: true } user ? user : null;

    public string Username => Principal?.Identity?.Name ?? Sistema;

    public int? UsuarioId =>
        int.TryParse(Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
