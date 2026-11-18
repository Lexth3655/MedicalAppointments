using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class MedicosRepository(AgendaContext context)
    : RepositoryGeneric<Medicos>(context), IMedicosRepository
{
    public Task<Medicos?> GetByIdWithEspecialidadAsync(long id, CancellationToken cancellationToken)
        => context.Medicos.AsNoTracking().Include(x => x.Especialidad)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
