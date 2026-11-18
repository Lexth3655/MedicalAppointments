using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class HorariosMedicosRepository(AgendaContext context)
    : RepositoryGeneric<HorariosMedicos>(context), IHorariosMedicosRepository
{
    public Task<HorariosMedicos?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken)
        => context.HorariosMedicos.AsNoTracking()
            .Include(x => x.Medico).Include(x => x.Consultorio)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<HorariosMedicos>> GetByMedicoAsync(long medicoId, CancellationToken cancellationToken)
        => await context.HorariosMedicos.AsNoTracking()
            .Include(x => x.Consultorio)
            .Where(x => x.MedicoId == medicoId && x.Activo)
            .OrderBy(x => x.FechaDisponible).ThenBy(x => x.HoraInicio)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<HorariosMedicos>> GetByMedicoAndDateAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken)
        => await context.HorariosMedicos.AsNoTracking()
            .Where(x => x.MedicoId == medicoId && x.FechaDisponible == fecha && x.Activo)
            .OrderBy(x => x.HoraInicio)
            .ToListAsync(cancellationToken);
}
