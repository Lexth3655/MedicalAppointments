using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Commands;

public sealed record CambiarEstadoCitaCommand(long Id, string Estado) : IRequest<bool>;

public sealed class CambiarEstadoCitaCommandHandler(IRepository<Citas> repository)
    : IRequestHandler<CambiarEstadoCitaCommand, bool>
{
    public async Task<bool> Handle(CambiarEstadoCitaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Estado = request.Estado;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
