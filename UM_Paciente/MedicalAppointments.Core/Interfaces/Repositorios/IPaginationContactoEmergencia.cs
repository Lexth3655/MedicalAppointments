using MedicalAppointments.Patients.Domain.Models;
using NugetClass.Abstractions;
using NugetClass.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Core.Interfaces.Repositorios
{
    public interface IPaginationContactoEmergencia: IPagedRepository<ContactoEmergencia> { }

}
