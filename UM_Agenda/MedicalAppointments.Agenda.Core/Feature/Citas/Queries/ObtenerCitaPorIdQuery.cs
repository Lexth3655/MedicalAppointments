using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Queries;

public sealed record ObtenerCitaPorIdQuery(long Id) : IRequest<Citas?>;

public sealed class ObtenerCitaPorIdQueryHandler(ICitasRepository repository)
    : IRequestHandler<ObtenerCitaPorIdQuery, Citas?>
{
    public Task<Citas?> Handle(ObtenerCitaPorIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
}
