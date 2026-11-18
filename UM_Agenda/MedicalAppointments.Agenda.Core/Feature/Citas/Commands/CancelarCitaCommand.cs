using MediatR;
using MedicalAppointments.Agenda.Domain.Enums;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Commands;

public sealed record CancelarCitaCommand(long Id) : IRequest<bool>;

public sealed class CancelarCitaCommandHandler(IRepository<Citas> repository)
    : IRequestHandler<CancelarCitaCommand, bool>
{
    public async Task<bool> Handle(CancelarCitaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Estado = EstadoCita.Cancelada.ToString();
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
