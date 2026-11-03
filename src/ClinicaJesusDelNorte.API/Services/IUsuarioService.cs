using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Services;

public interface IUsuarioService
{
    PagedResult<UsuarioResponseDto> Listar(int page, int size);
    UsuarioResponseDto ObtenerPorId(int id);
    UsuarioResponseDto Crear(CrearUsuarioDto dto);
    UsuarioResponseDto Actualizar(int id, ActualizarUsuarioDto dto);
    UsuarioResponseDto Desbloquear(int id);
    UsuarioResponseDto CambiarContrasena(int id, CambiarContrasenaDto dto);
    IReadOnlyList<RolResponseDto> ListarRoles();
}

public interface IAuthService
{
    LoginResponseDto Login(LoginDto dto);
    UsuarioResponseDto Perfil();
}
