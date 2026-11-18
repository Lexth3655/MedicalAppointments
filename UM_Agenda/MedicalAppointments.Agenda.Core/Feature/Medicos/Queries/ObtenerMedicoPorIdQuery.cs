using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;

namespace MedicalAppointments.Agenda.Core.Feature.Medico.Queries;

public sealed record ObtenerMedicoPorIdQuery(long Id) : IRequest<Medicos?>;

public sealed class ObtenerMedicoPorIdQueryHandler(IMedicosRepository repository)
    : IRequestHandler<ObtenerMedicoPorIdQuery, Medicos?>
{
    public Task<Medicos?> Handle(ObtenerMedicoPorIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdWithEspecialidadAsync(request.Id, cancellationToken);
}
