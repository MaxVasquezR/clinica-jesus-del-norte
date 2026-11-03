using System.Security.Claims;
using System.Text;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClinicaJesusDelNorte.API.Seguridad;

/// <summary>
/// Configuración del token (sección "Jwt" de appsettings.json).
/// </summary>
public class JwtOptions
{
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public int MinutosExpiracion { get; set; } = 480;

    public SymmetricSecurityKey ClaveDeFirma() => new(Encoding.UTF8.GetBytes(Clave));
}

public interface ITokenService
{
    (string Token, DateTime Expira) Generar(Usuario usuario, TipoRol rol);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(JwtOptions options)
    {
        _options = options;
    }

    public (string Token, DateTime Expira) Generar(Usuario usuario, TipoRol rol)
    {
        var expira = DateTime.UtcNow.AddMinutes(_options.MinutosExpiracion);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, rol.ToString())
            ]),
            Issuer = _options.Emisor,
            Audience = _options.Audiencia,
            Expires = expira,
            SigningCredentials = new SigningCredentials(_options.ClaveDeFirma(), SecurityAlgorithms.HmacSha256)
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expira);
    }
}
