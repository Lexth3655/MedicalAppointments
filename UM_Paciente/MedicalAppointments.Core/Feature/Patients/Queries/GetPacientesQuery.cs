using MediatR;
using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;

namespace MedicalAppointments.Patients.Core.Feature.Patients.Queries
{
    public class GetPacientesQuery : IRequest<IReadOnlyList<Paciente>> { }

    public class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, IReadOnlyList<Paciente>>
    {
        private readonly IPaciente _pacienteRepository;

        public GetPacientesQueryHandler(IPaciente pacienteRepository)
        {
            _pacienteRepository = pacienteRepository ?? throw new ArgumentNullException(nameof(pacienteRepository));
        }

        public async Task<IReadOnlyList<Paciente>> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
        {
            var all = await _pacienteRepository.GetAllAsync(cancellationToken);
            return all.Where(p => p.Activo).ToList();
        }
    }
}
