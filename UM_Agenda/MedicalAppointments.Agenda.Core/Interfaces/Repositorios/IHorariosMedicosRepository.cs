using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Interfaces.Repositorios;

public interface IHorariosMedicosRepository : IRepository<HorariosMedicos>
{
    Task<HorariosMedicos?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<HorariosMedicos>> GetByMedicoAsync(long medicoId, CancellationToken cancellationToken);
    Task<IReadOnlyList<HorariosMedicos>> GetByMedicoAndDateAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken);
}
