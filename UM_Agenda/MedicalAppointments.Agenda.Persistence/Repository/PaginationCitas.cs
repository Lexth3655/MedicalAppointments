using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class PaginationCitas(AgendaContext context)
    : PagedRepository<Citas, AgendaContext>(context), IPaginationCitas
{
    protected override string DefaultSortField => nameof(Citas.Id);
}
