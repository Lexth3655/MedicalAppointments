using MediatR;
using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Queries;

public sealed record ObtenerHorarioPorIdQuery(long Id) : IRequest<HorariosMedicos?>;

public sealed class ObtenerHorarioPorIdQueryHandler(IHorariosMedicosRepository repository)
    : IRequestHandler<ObtenerHorarioPorIdQuery, HorariosMedicos?>
{
    public Task<HorariosMedicos?> Handle(ObtenerHorarioPorIdQuery request, CancellationToken cancellationToken)
        => repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
}
