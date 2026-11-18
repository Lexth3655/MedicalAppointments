using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class PaginationConsultorios(AgendaContext context)
    : PagedRepository<Consultorios, AgendaContext>(context), IPaginationConsultorios
{
    protected override string DefaultSortField => nameof(Consultorios.Id);
}
