using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Especialidad.Commands;

public sealed record ActualizarEspecialidadCommand(long Id, string Nombre, string? Descripcion) : IRequest<bool>;

public sealed class ActualizarEspecialidadCommandHandler(IRepository<Especialidades> repository)
    : IRequestHandler<ActualizarEspecialidadCommand, bool>
{
    public async Task<bool> Handle(ActualizarEspecialidadCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Nombre = request.Nombre;
        entity.Descripcion = request.Descripcion;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
