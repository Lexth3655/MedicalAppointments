using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Domain.Models;
using MedicalAppointments.Agenda.Persistence.Data;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence.Repository;

public sealed class PaginationHorariosMedicos(AgendaContext context)
    : PagedRepository<HorariosMedicos, AgendaContext>(context), IPaginationHorariosMedicos
{
    protected override string DefaultSortField => nameof(HorariosMedicos.Id);
}
