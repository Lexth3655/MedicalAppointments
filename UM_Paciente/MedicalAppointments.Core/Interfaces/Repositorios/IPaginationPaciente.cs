using MedicalAppointments.Patients.Domain.Models;
using NugetClass.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Core.Interfaces.Repositorios
{
    public interface IPaginationPaciente : IPagedRepository<Paciente>
    {
    }
}
