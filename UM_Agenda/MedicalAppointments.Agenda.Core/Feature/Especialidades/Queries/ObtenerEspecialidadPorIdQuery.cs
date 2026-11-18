using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Especialidad.Queries;

public sealed record ObtenerEspecialidadPorIdQuery(long Id) : IRequest<Especialidades?>;

public sealed class ObtenerEspecialidadPorIdQueryHandler(IRepository<Especialidades> repository)
    : IRequestHandler<ObtenerEspecialidadPorIdQuery, Especialidades?>
{
    public Task<Especialidades?> Handle(ObtenerEspecialidadPorIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdAsync(request.Id, cancellationToken);
}
