using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class CitasRepository(AgendaContext context)
    : RepositoryGeneric<Citas>(context), ICitasRepository
{
    public Task<Citas?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken)
        => context.Citas.AsNoTracking()
            .Include(x => x.Medico).ThenInclude(x => x.Especialidad)
            .Include(x => x.Consultorio)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Citas>> GetByPacienteAsync(long pacienteId, CancellationToken cancellationToken)
        => await context.Citas.AsNoTracking()
            .Include(x => x.Medico).ThenInclude(x => x.Especialidad)
            .Include(x => x.Consultorio)
            .Where(x => x.PacienteId == pacienteId && x.Activo)
            .OrderBy(x => x.FechaHora)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Citas>> GetAgendaDelDiaAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken)
    {
        var start = fecha.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);
        return await context.Citas.AsNoTracking()
            .Include(x => x.Medico).ThenInclude(x => x.Especialidad)
            .Include(x => x.Consultorio)
            .Where(x => x.MedicoId == medicoId && x.FechaHora >= start && x.FechaHora < end &&
                x.Activo && x.Estado != "Cancelada")
            .OrderBy(x => x.FechaHora)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOnly>> GetBookedTimesAsync(long medicoId, DateOnly fecha, CancellationToken cancellationToken)
    {
        var start = fecha.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);
        var booked = await context.Citas.AsNoTracking()
            .Where(x => x.MedicoId == medicoId && x.FechaHora >= start && x.FechaHora < end &&
                x.Activo && x.Estado != "Cancelada")
            .Select(x => x.FechaHora)
            .ToListAsync(cancellationToken);
        return booked.Select(DateOnlyTime).ToArray();
    }

    private static TimeOnly DateOnlyTime(DateTime value) => TimeOnly.FromDateTime(value);
}
