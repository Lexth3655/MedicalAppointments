using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;

public sealed record ActualizarHorarioCommand(long Id, long MedicoId, long ConsultorioId,
    DateOnly FechaDisponible, TimeOnly HoraInicio, TimeOnly HoraFin, int DuracionCitaMin) : IRequest<bool>;

public sealed class ActualizarHorarioCommandHandler(IRepository<HorariosMedicos> repository)
    : IRequestHandler<ActualizarHorarioCommand, bool>
{
    public async Task<bool> Handle(ActualizarHorarioCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.MedicoId = request.MedicoId;
        entity.ConsultorioId = request.ConsultorioId;
        entity.FechaDisponible = request.FechaDisponible;
        entity.HoraInicio = request.HoraInicio;
        entity.HoraFin = request.HoraFin;
        entity.DuracionCitaMin = request.DuracionCitaMin;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
