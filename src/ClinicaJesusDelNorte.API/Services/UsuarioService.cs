using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;
using ClinicaJesusDelNorte.API.Seguridad;
using Microsoft.AspNetCore.Identity;

namespace ClinicaJesusDelNorte.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IRepositorio<Rol> _roles;
    private readonly IRepositorio<Medico> _medicos;
    private readonly IPersonaService _personaService;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly IUsuarioActual _usuarioActual;

    public UsuarioService(
        IRepositorio<Usuario> usuarios,
        IRepositorio<Rol> roles,
        IRepositorio<Medico> medicos,
        IPersonaService personaService,
        IPasswordHasher<Usuario> hasher,
        IUsuarioActual usuarioActual)
    {
        _usuarios = usuarios;
        _roles = roles;
        _medicos = medicos;
        _personaService = personaService;
        _hasher = hasher;
        _usuarioActual = usuarioActual;
    }

    public PagedResult<UsuarioResponseDto> Listar(int page, int size)
    {
        return Paginador.Crear(_usuarios.GetAll(), page, size, Map);
    }

    public UsuarioResponseDto ObtenerPorId(int id)
    {
        return Map(ObtenerUsuario(id));
    }

    public UsuarioResponseDto Crear(CrearUsuarioDto dto)
    {
        var rol = ObtenerRolActivo(dto.RolId);
        var username = dto.Username.Trim().ToLowerInvariant();

        if (_usuarios.Any(u => u.Username == username))
            throw new ConflictException($"El nombre de usuario '{username}' ya existe.");

        var existente = _personaService.BuscarPorDocumento(dto.Persona.TipoDocumento, dto.Persona.NumeroDocumento);
        if (existente is not null && _usuarios.FirstOrDefault(u => u.PersonaId == existente.Id) is { } otro)
            throw new ConflictException($"La persona con documento {existente.NumeroDocumento} ya tiene el usuario '{otro.Username}'.");

        ValidarRolMedico(rol, existente?.Id);

        var persona = _personaService.ObtenerOCrear(dto.Persona);
        var usuario = new Usuario { PersonaId = persona.Id, RolId = rol.Id, Username = username, Activo = true };
        usuario.ContrasenaHash = _hasher.HashPassword(usuario, dto.Contrasena);

        return Map(_usuarios.Add(usuario));
    }

    public UsuarioResponseDto Actualizar(int id, ActualizarUsuarioDto dto)
    {
        var usuario = ObtenerUsuario(id);
        var rol = ObtenerRolActivo(dto.RolId);
        ValidarRolMedico(rol, usuario.PersonaId);

        if (!dto.Activo && _usuarioActual.UsuarioId == id)
            throw new BusinessException("No puede desactivar su propio usuario.");

        usuario.RolId = rol.Id;
        usuario.Activo = dto.Activo;
        return Map(_usuarios.Update(usuario));
    }

    public UsuarioResponseDto Desbloquear(int id)
    {
        var usuario = ObtenerUsuario(id);
        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHasta = null;
        return Map(_usuarios.Update(usuario));
    }

    public UsuarioResponseDto CambiarContrasena(int id, CambiarContrasenaDto dto)
    {
        var usuario = ObtenerUsuario(id);
        usuario.ContrasenaHash = _hasher.HashPassword(usuario, dto.Contrasena);
        usuario.IntentosFallidos = 0;
        usuario.BloqueadoHasta = null;
        return Map(_usuarios.Update(usuario));
    }

    public IReadOnlyList<RolResponseDto> ListarRoles()
    {
        return _roles.GetAll().Select(r => new RolResponseDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion,
            Activo = r.Activo
        }.ConAuditoria(r)).ToList();
    }

    private Usuario ObtenerUsuario(int id) =>
        _usuarios.GetById(id) ?? throw new NotFoundException($"No se encontró el usuario con ID {id}.");

    private Rol ObtenerRolActivo(int rolId)
    {
        var rol = _roles.GetById(rolId);
        if (rol is null || !rol.Activo)
            throw new BusinessException($"El rol con ID {rolId} no existe o está inactivo.");
        return rol;
    }

    // Un usuario con rol Medico debe corresponder a una persona registrada como médico.
    private void ValidarRolMedico(Rol rol, int? personaId)
    {
        if (rol.Nombre == TipoRol.Medico && (personaId is null || !_medicos.Any(m => m.PersonaId == personaId)))
            throw new BusinessException("Para el rol Medico, la persona debe estar registrada antes como médico.");
    }

    private UsuarioResponseDto Map(Usuario u) => new UsuarioResponseDto
    {
        Id = u.Id,
        Username = u.Username,
        RolId = u.RolId,
        Rol = _roles.GetById(u.RolId)!.Nombre,
        Activo = u.Activo,
        IntentosFallidos = u.IntentosFallidos,
        BloqueadoHasta = u.BloqueadoHasta,
        UltimoAcceso = u.UltimoAcceso,
        Persona = _personaService.Map(u.PersonaId)
    }.ConAuditoria(u);
}
