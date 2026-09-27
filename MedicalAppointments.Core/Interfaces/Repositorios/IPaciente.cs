using MedicalAppointments.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Core.Interfaces.Repositorios
{
    public interface IPaciente: IRepository<Paciente>
    {
        Task<string> GenerateNextCodigoAsync(CancellationToken cancellationToken);
    }
}
