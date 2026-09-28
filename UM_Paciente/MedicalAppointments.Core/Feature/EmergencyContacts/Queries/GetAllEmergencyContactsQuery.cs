using MediatR;
using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;

namespace MedicalAppointments.Patients.Core.Feature.EmergencyContacts.Queries
{
    public sealed class GetAllEmergencyContactsQuery : IRequest<IReadOnlyList<ContactoEmergencia>> { }

    public sealed class GetAllEmergencyContactsQueryHandler(IContactoEmergencia contactsRepository) : IRequestHandler<GetAllEmergencyContactsQuery, IReadOnlyList<ContactoEmergencia>>
    {
        public Task<IReadOnlyList<ContactoEmergencia>> Handle(GetAllEmergencyContactsQuery request, CancellationToken cancellationToken)
            => contactsRepository.GetAllAsync(cancellationToken);
    }

    public sealed class GetEmergencyContactsByPacienteQuery : IRequest<IReadOnlyList<ContactoEmergencia>>
    {
        public long PacienteId { get; set; }
    }

    public sealed class GetEmergencyContactsByPacienteQueryHandler(
        IPaciente patientsRepository,
        IContactoEmergencia contactsRepository) : IRequestHandler<GetEmergencyContactsByPacienteQuery, IReadOnlyList<ContactoEmergencia>>
    {
        public async Task<IReadOnlyList<ContactoEmergencia>> Handle(GetEmergencyContactsByPacienteQuery request, CancellationToken cancellationToken)
        {
            if (await patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken) is null)
                throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");
            return await contactsRepository.GetByFilterAsync(c => c.PacienteId == request.PacienteId, cancellationToken: cancellationToken);
        }
    }
}
