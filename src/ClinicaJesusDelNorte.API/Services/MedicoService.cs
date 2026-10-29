using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using ClinicaJesusDelNorte.API.Models.DTOs;
using ClinicaJesusDelNorte.API.Models.Entities;
using ClinicaJesusDelNorte.API.Repository;

namespace ClinicaJesusDelNorte.API.Services;

public class MedicoService : IMedicoService
{
    private readonly IMedicoRepository _repository;
    private readonly IEspecialidadRepository _especialidadRepository;

    public MedicoService(IMedicoRepository repository, IEspecialidadRepository especialidadRepository)
    {
        _repository = repository;
        _especialidadRepository = especialidadRepository;
    }

    public PagedResult<MedicoResponseDto> Listar(int page, int size)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, 100);

        var all = _repository.GetAll();
        var total = all.Count;
        var items = all.Skip(page * size).Take(size).Select(Map).ToList();

        return new PagedResult<MedicoResponseDto>
        {
            Items = items,
            Page = page,
            Size = size,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)size)
        };
    }

    public MedicoResponseDto ObtenerPorId(int id)
    {
        var medico = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró el médico con ID {id}.");
        return Map(medico);
    }

    public MedicoResponseDto Crear(CrearMedicoDto dto)
    {
        ValidarEspecialidad(dto.EspecialidadId);

        if (_repository.GetByDocumento(dto.NumeroDocumento) is not null)
        {
            throw new ConflictException($"Ya existe un médico con documento {dto.NumeroDocumento}.");
        }

        if (_repository.GetByColegiatura(dto.CodigoColegiatura) is not null)
        {
            throw new ConflictException($"Ya existe un médico con colegiatura {dto.CodigoColegiatura}.");
        }

        var medico = new Medico
        {
            TipoDocumento = dto.TipoDocumento,
            NumeroDocumento = dto.NumeroDocumento.Trim(),
            Nombres = dto.Nombres.Trim(),
            Apellidos = dto.Apellidos.Trim(),
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Direccion = dto.Direccion,
            CodigoColegiatura = dto.CodigoColegiatura.Trim(),
            Rne = dto.Rne,
            EspecialidadId = dto.EspecialidadId,
            Activo = dto.Activo
        };

        return Map(_repository.Add(medico));
    }

    public MedicoResponseDto Actualizar(int id, ActualizarMedicoDto dto)
    {
        var medico = _repository.GetById(id)
            ?? throw new NotFoundException($"No se encontró el médico con ID {id}.");

        ValidarEspecialidad(dto.EspecialidadId);

        medico.Nombres = dto.Nombres.Trim();
        medico.Apellidos = dto.Apellidos.Trim();
        medico.FechaNacimiento = dto.FechaNacimiento;
        medico.Telefono = dto.Telefono;
        medico.Email = dto.Email;
        medico.Direccion = dto.Direccion;
        medico.Rne = dto.Rne;
        medico.EspecialidadId = dto.EspecialidadId;
        medico.Activo = dto.Activo;

        return Map(_repository.Update(medico));
    }

    public void Eliminar(int id)
    {
        if (!_repository.SoftDelete(id))
        {
            throw new NotFoundException($"No se encontró el médico con ID {id}.");
        }
    }

    private void ValidarEspecialidad(int especialidadId)
    {
        var especialidad = _especialidadRepository.GetById(especialidadId);
        if (especialidad is null || !especialidad.Activo)
        {
            throw new BusinessException($"La especialidad con ID {especialidadId} no existe o está inactiva.");
        }
    }

    private MedicoResponseDto Map(Medico m)
    {
        var especialidad = _especialidadRepository.GetById(m.EspecialidadId);
        return new MedicoResponseDto
        {
            Id = m.Id,
            CodigoColegiatura = m.CodigoColegiatura,
            Rne = m.Rne,
            TipoDocumento = m.TipoDocumento,
            NumeroDocumento = m.NumeroDocumento,
            Nombres = m.Nombres,
            Apellidos = m.Apellidos,
            NombreCompleto = m.NombreCompleto,
            FechaNacimiento = m.FechaNacimiento,
            Telefono = m.Telefono,
            Email = m.Email,
            Direccion = m.Direccion,
            EspecialidadId = m.EspecialidadId,
            EspecialidadNombre = especialidad?.Nombre,
            Activo = m.Activo,
            FechaRegistro = m.FechaRegistro
        };
    }
}
