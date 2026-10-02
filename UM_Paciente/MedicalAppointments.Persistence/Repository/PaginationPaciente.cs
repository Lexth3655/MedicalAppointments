using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using MedicalAppointments.Patients.Persistence.Data;
using NugetClass.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Persistence.Repository
{
    internal class PaginationPaciente : PagedRepository<Paciente, AppDbContext>, IPaginationPaciente
    {
        public PaginationPaciente(AppDbContext context) : base(context)
        {
        }

        // 🔑 Aquí está la clave: la PK real de Paciente
        protected override string DefaultSortField => nameof(Paciente.PacienteId);
    }
}
