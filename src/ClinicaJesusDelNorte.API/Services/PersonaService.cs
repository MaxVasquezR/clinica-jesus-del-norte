using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Models.Enums;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

/// <summary>
/// Una persona se registra una sola vez por documento y se reutiliza como paciente, médico o usuario.
/// </summary>
public class PersonaService : IPersonaService
{
    private readonly IRepositorio<Persona> _personas;

    public PersonaService(IRepositorio<Persona> personas)
    {
        _personas = personas;
    }

    public Persona? BuscarPorDocumento(TipoDocumento tipoDocumento, string numeroDocumento)
    {
        var numero = numeroDocumento.Trim();
        return _personas.FirstOrDefault(p => p.TipoDocumento == tipoDocumento && p.NumeroDocumento == numero);
    }

    public Persona ObtenerOCrear(DatosPersonaDto dto)
    {
        var existente = BuscarPorDocumento(dto.TipoDocumento, dto.NumeroDocumento);
        if (existente is not null)
        {
            return existente;
        }

        var persona = new Persona
        {
            TipoDocumento = dto.TipoDocumento,
            NumeroDocumento = dto.NumeroDocumento.Trim()
        };
        Asignar(persona, dto);
        return _personas.Add(persona);
    }

    public Persona Actualizar(int personaId, ActualizarPersonaDto dto)
    {
        var persona = _personas.GetById(personaId)
            ?? throw new NotFoundException($"No se encontró la persona con ID {personaId}.");
        Asignar(persona, dto);
        return _personas.Update(persona);
    }

    public PersonaResponseDto Map(int personaId)
    {
        var p = _personas.GetById(personaId)
            ?? throw new NotFoundException($"No se encontró la persona con ID {personaId}.");

        return new PersonaResponseDto
        {
            Id = p.Id,
            TipoDocumento = p.TipoDocumento,
            NumeroDocumento = p.NumeroDocumento,
            Nombres = p.Nombres,
            Apellidos = p.Apellidos,
            NombreCompleto = p.NombreCompleto,
            FechaNacimiento = p.FechaNacimiento,
            Sexo = p.Sexo,
            Telefono = p.Telefono,
            Email = p.Email,
            Direccion = p.Direccion
        }.ConAuditoria(p);
    }

    private static void Asignar(Persona persona, ActualizarPersonaDto dto)
    {
        persona.Nombres = dto.Nombres.Trim();
        persona.Apellidos = dto.Apellidos.Trim();
        persona.FechaNacimiento = dto.FechaNacimiento;
        persona.Sexo = dto.Sexo;
        persona.Telefono = Texto.Limpiar(dto.Telefono);
        persona.Email = Texto.Limpiar(dto.Email);
        persona.Direccion = Texto.Limpiar(dto.Direccion);
    }
}
