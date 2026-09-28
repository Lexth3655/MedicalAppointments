using MediatR;
using MedicalAppointments.Patients.Core.Interfaces.Repositorios;

namespace MedicalAppointments.Patients.Core.Feature.Patients.Command
{
    public class DeletePacienteCommand : IRequest<bool>
    {
        public long PacienteId { get; set; }
    }

    public class DeletePacienteCommandHandler(IPaciente patientsRepository) : IRequestHandler<DeletePacienteCommand, bool>
    {
        public Task<bool> Handle(DeletePacienteCommand request, CancellationToken cancellationToken)
            => patientsRepository.DeleteAsync(request.PacienteId, cancellationToken);
    }
}
