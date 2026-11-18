using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class PaginationMedicos(AgendaContext context)
    : PagedRepository<Medicos, AgendaContext>(context), IPaginationMedicos
{
    protected override string DefaultSortField => nameof(Medicos.Id);
}
