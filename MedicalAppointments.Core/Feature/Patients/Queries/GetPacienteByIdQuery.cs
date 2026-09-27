using MediatR;
using MedicalAppointments.Core.Interfaces.Repositorios;
using MedicalAppointments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Core.Feature.Patients.Queries
{
    public class GetPacienteByIdQuery : IRequest<Paciente?>
    {
        public long PacienteId { get; set; }
    }

    public class GetPacienteByIdQueryHandler(IPaciente patientsRepository) : IRequestHandler<GetPacienteByIdQuery, Paciente?>
    {
        public Task<Paciente?> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
            => patientsRepository.GetByIdAsync(request.PacienteId, cancellationToken);
    }
}
