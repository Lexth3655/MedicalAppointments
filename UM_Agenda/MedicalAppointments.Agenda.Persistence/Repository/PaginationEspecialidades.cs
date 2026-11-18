using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class PaginationEspecialidades(AgendaContext context)
    : PagedRepository<Especialidades, AgendaContext>(context), IPaginationEspecialidades
{
    protected override string DefaultSortField => nameof(Especialidades.Id);
}
