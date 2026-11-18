using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Consultorio.Queries;

public sealed record ObtenerConsultorioPorIdQuery(long Id) : IRequest<Consultorios?>;

public sealed class ObtenerConsultorioPorIdQueryHandler(IRepository<Consultorios> repository)
    : IRequestHandler<ObtenerConsultorioPorIdQuery, Consultorios?>
{
    public Task<Consultorios?> Handle(ObtenerConsultorioPorIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdAsync(request.Id, cancellationToken);
}
