using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;

public sealed record CambiarEstadoHorarioCommand(long Id, bool Activo) : IRequest<bool>;

public sealed class CambiarEstadoHorarioCommandHandler(IRepository<HorariosMedicos> repository)
    : IRequestHandler<CambiarEstadoHorarioCommand, bool>
{
    public async Task<bool> Handle(CambiarEstadoHorarioCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Activo = request.Activo;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
