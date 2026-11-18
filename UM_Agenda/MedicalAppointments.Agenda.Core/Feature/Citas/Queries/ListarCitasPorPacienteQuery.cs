using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Queries;

public sealed record ListarCitasPorPacienteQuery(long PacienteId) : IRequest<IReadOnlyList<Citas>>;

public sealed class ListarCitasPorPacienteQueryHandler(ICitasRepository repository)
    : IRequestHandler<ListarCitasPorPacienteQuery, IReadOnlyList<Citas>>
{
    public Task<IReadOnlyList<Citas>> Handle(ListarCitasPorPacienteQuery request, CancellationToken cancellationToken)
        => repository.GetByPacienteAsync(request.PacienteId, cancellationToken);
}
