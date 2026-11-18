using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Queries;

public sealed record ObtenerDisponibilidadQuery(long MedicoId, DateOnly Fecha) : IRequest<IReadOnlyList<TimeOnly>>;

public sealed class ObtenerDisponibilidadQueryHandler(
    IHorariosMedicosRepository horariosRepository,
    ICitasRepository citasRepository)
    : IRequestHandler<ObtenerDisponibilidadQuery, IReadOnlyList<TimeOnly>>
{
    public async Task<IReadOnlyList<TimeOnly>> Handle(ObtenerDisponibilidadQuery request, CancellationToken cancellationToken)
    {
        var schedules = await horariosRepository.GetByMedicoAndDateAsync(
            request.MedicoId, request.Fecha, cancellationToken);
        var bookedTimes = (await citasRepository.GetBookedTimesAsync(
            request.MedicoId, request.Fecha, cancellationToken)).ToHashSet();
        var available = new SortedSet<TimeOnly>();

        foreach (var schedule in schedules)
        {
            if (schedule.DuracionCitaMin <= 0 || schedule.HoraInicio >= schedule.HoraFin)
                continue;

            var slot = schedule.HoraInicio;
            while (slot.AddMinutes(schedule.DuracionCitaMin) <= schedule.HoraFin)
            {
                if (!bookedTimes.Contains(slot))
                    available.Add(slot);
                slot = slot.AddMinutes(schedule.DuracionCitaMin);
            }
        }

        return available.ToArray();
    }
}
