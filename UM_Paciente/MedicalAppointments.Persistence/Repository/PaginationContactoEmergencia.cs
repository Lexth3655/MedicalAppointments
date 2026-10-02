using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using MedicalAppointments.Patients.Persistence.Data;
using NugetClass.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Persistence.Repository;

public class PaginationContactoEmergencia: PagedRepository<ContactoEmergencia, AppDbContext>, IPaginationContactoEmergencia
{
    public PaginationContactoEmergencia(AppDbContext context) : base(context) { }
    protected override string DefaultSortField => nameof(ContactoEmergencia.ContactoEmergenciaId);


}
