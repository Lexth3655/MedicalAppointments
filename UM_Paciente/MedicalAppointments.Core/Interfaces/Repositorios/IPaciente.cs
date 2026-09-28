using MedicalAppointments.Patients.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Patients.Core.Interfaces.Repositorios
{
    public interface IPaciente : IRepository<Paciente>
    {
        Task<string> GenerateNextCodigoAsync(CancellationToken cancellationToken);
    }
}
