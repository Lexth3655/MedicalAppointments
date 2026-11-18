using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Medico.Commands;

public sealed record CambiarEstadoMedicoCommand(long Id, bool Activo) : IRequest<bool>;

public sealed class CambiarEstadoMedicoCommandHandler(IRepository<Medicos> repository)
    : IRequestHandler<CambiarEstadoMedicoCommand, bool>
{
    public async Task<bool> Handle(CambiarEstadoMedicoCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Activo = request.Activo;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
