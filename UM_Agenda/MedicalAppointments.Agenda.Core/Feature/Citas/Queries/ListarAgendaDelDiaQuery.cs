using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Queries;

public sealed record ListarAgendaDelDiaQuery(long MedicoId, DateOnly Fecha) : IRequest<IReadOnlyList<Citas>>;

public sealed class ListarAgendaDelDiaQueryHandler(ICitasRepository repository)
    : IRequestHandler<ListarAgendaDelDiaQuery, IReadOnlyList<Citas>>
{
    public Task<IReadOnlyList<Citas>> Handle(ListarAgendaDelDiaQuery request, CancellationToken cancellationToken)
        => repository.GetAgendaDelDiaAsync(request.MedicoId, request.Fecha, cancellationToken);
}
