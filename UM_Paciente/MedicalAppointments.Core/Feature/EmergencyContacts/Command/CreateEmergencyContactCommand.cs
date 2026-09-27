using MediatR;
using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Core.Feature.EmergencyContacts.Command
{
    public class CreateEmergencyContactCommand : IRequest<ContactoEmergencia>
    {
        public long PacienteId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Parentesco { get; set; }
        public string Telefono { get; set; } = null!;
        public string? TelefonoSecundario { get; set; }
        public string? Email { get; set; }
        public int Prioridad { get; set; } = 1;
        public bool Activo { get; set; } = true;
    }

    public class CreateEmergencyContactCommandHandler : IRequestHandler<CreateEmergencyContactCommand, ContactoEmergencia>
    {
        private readonly IContactoEmergencia _repositoryEmergency;
        private readonly IPaciente _repositoryPaciente;

        public CreateEmergencyContactCommandHandler(IContactoEmergencia contactsRepository, IPaciente patientsRepository)
        {
            this._repositoryEmergency = contactsRepository;
            this._repositoryPaciente = patientsRepository;
        }

        public async Task<ContactoEmergencia> Handle(CreateEmergencyContactCommand request, CancellationToken cancellationToken)
        {
            if (await _repositoryPaciente.GetByIdAsync(request.PacienteId, cancellationToken) is null)
                throw new KeyNotFoundException($"Paciente {request.PacienteId} no encontrado.");

            var contacto = new ContactoEmergencia
            {
                PacienteId = request.PacienteId,
                NombreCompleto = request.NombreCompleto,
                Parentesco = request.Parentesco,
                Telefono = request.Telefono,
                TelefonoSecundario = request.TelefonoSecundario,
                Email = request.Email,
                Prioridad = request.Prioridad,
                Activo = request.Activo
            };

            await _repositoryEmergency.AddAsync(contacto, cancellationToken);
            return contacto;
        }
    }
}
