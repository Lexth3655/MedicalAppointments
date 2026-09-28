using MedicalAppointments.Patients.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Patients.Core.Interfaces.Repositorios
{
    public interface IContactoEmergencia : IRepository<ContactoEmergencia>
    {
        Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default);
    }
}
