using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Especialidad.Commands;

public sealed record CambiarEstadoEspecialidadCommand(long Id, bool Activo) : IRequest<bool>;

public sealed class CambiarEstadoEspecialidadCommandHandler(IRepository<Especialidades> repository)
    : IRequestHandler<CambiarEstadoEspecialidadCommand, bool>
{
    public async Task<bool> Handle(CambiarEstadoEspecialidadCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Activo = request.Activo;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
