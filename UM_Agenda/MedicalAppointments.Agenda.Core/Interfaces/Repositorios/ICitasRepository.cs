using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Interfaces.Repositorios;

public interface ICitasRepository : IRepository<Citas>
{
    Task<Citas?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Citas>> GetByPacienteAsync(long pacienteId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Citas>> GetAgendaDelDiaAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken);
    Task<IReadOnlyList<TimeOnly>> GetBookedTimesAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken);
}
