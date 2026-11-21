using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;

public sealed record ActualizarEstadoTurnoCommand(long Id, string EstadoTurno) : IRequest<bool>;

public sealed class ActualizarEstadoTurnoCommandHandler(IRepository<HorariosMedicos> repository)
    : IRequestHandler<ActualizarEstadoTurnoCommand, bool>
{
    private static readonly HashSet<string> Allowed = new(["Programado", "EnTurno", "EnDescanso", "Break", "Finalizado"], StringComparer.OrdinalIgnoreCase);

    public async Task<bool> Handle(ActualizarEstadoTurnoCommand request, CancellationToken cancellationToken)
    {
        if (!Allowed.Contains(request.EstadoTurno)) return false;
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.EstadoTurno = request.EstadoTurno;
        entity.Activo = request.EstadoTurno.Equals("EnTurno", StringComparison.OrdinalIgnoreCase) || request.EstadoTurno.Equals("Programado", StringComparison.OrdinalIgnoreCase);
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
