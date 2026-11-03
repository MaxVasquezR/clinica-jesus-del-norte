using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;
using ClinicaJesusDelNorte.API.Seguridad;
using Microsoft.AspNetCore.Identity;

namespace ClinicaJesusDelNorte.API.Services;

public class AuthService : IAuthService
{
    private const int MaximoIntentos = 3;
    private const int MinutosBloqueo = 15;
    private const string CredencialesInvalidas = "Usuario o contraseña incorrectos.";

    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IRepositorio<Rol> _roles;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly ITokenService _tokens;
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioActual _usuarioActual;

    public AuthService(
        IRepositorio<Usuario> usuarios,
        IRepositorio<Rol> roles,
        IPasswordHasher<Usuario> hasher,
        ITokenService tokens,
        IUsuarioService usuarioService,
        IUsuarioActual usuarioActual)
    {
        _usuarios = usuarios;
        _roles = roles;
        _hasher = hasher;
        _tokens = tokens;
        _usuarioService = usuarioService;
        _usuarioActual = usuarioActual;
    }

    public LoginResponseDto Login(LoginDto dto)
    {
        var username = dto.Username.Trim().ToLowerInvariant();
        var usuario = _usuarios.FirstOrDefault(u => u.Username == username)
            ?? throw new UnauthorizedException(CredencialesInvalidas);

        if (!usuario.Activo)
            throw new UnauthorizedException("El usuario está desactivado. Comuníquese con Administración.");

        var ahora = DateTime.UtcNow;
        if (usuario.BloqueadoHasta > ahora)
            throw new UnauthorizedException(
                $"Usuario bloqueado por intentos fallidos hasta las {usuario.BloqueadoHasta.Value.ToLocalTime():HH:mm}. Administración puede desbloquearlo.");

        if (usuario.BloqueadoHasta is not null)
        {
            // El bloqueo venció: el usuario vuelve a tener todos sus intentos.
            usuario.BloqueadoHasta = null;
            usuario.IntentosFallidos = 0;
        }

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, dto.Contrasena);
        if (resultado == PasswordVerificationResult.Failed)
        {
            usuario.IntentosFallidos++;
            var bloqueado = usuario.IntentosFallidos >= MaximoIntentos;
            if (bloqueado)
                usuario.BloqueadoHasta = ahora.AddMinutes(MinutosBloqueo);
            _usuarios.Update(usuario);

            throw new UnauthorizedException(bloqueado
                ? $"Tercer intento fallido: el usuario queda bloqueado {MinutosBloqueo} minutos."
                : $"{CredencialesInvalidas} Le quedan {MaximoIntentos - usuario.IntentosFallidos} intento(s).");
        }

        var rol = _roles.GetById(usuario.RolId)!;
        if (!rol.Activo)
            throw new UnauthorizedException("El rol del usuario está inactivo. Comuníquese con Administración.");

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            usuario.ContrasenaHash = _hasher.HashPassword(usuario, dto.Contrasena);

        usuario.IntentosFallidos = 0;
        usuario.UltimoAcceso = ahora;
        _usuarios.Update(usuario);

        var (token, expira) = _tokens.Generar(usuario, rol.Nombre);
        return new LoginResponseDto
        {
            Token = token,
            Expira = expira,
            Usuario = _usuarioService.ObtenerPorId(usuario.Id)
        };
    }

    public UsuarioResponseDto Perfil()
    {
        var id = _usuarioActual.UsuarioId ?? throw new UnauthorizedException("Debe iniciar sesión.");
        return _usuarioService.ObtenerPorId(id);
    }
}
