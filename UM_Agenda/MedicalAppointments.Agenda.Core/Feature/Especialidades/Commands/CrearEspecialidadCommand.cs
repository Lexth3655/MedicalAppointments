using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Especialidad.Commands;

public sealed class CrearEspecialidadCommand : IRequest<Especialidades>
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public sealed class CrearEspecialidadCommandHandler(IRepository<Especialidades> repository)
    : IRequestHandler<CrearEspecialidadCommand, Especialidades>
{
    public async Task<Especialidades> Handle(CrearEspecialidadCommand request, CancellationToken cancellationToken)
    {
        var entity = new Especialidades { Nombre = request.Nombre, Descripcion = request.Descripcion, Activo = true };
        await repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}
