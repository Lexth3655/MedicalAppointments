using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Consultorio.Commands;

public sealed record CambiarEstadoConsultorioCommand(long Id, bool Activo) : IRequest<bool>;

public sealed class CambiarEstadoConsultorioCommandHandler(IRepository<Consultorios> repository)
    : IRequestHandler<CambiarEstadoConsultorioCommand, bool>
{
    public async Task<bool> Handle(CambiarEstadoConsultorioCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Activo = request.Activo;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
