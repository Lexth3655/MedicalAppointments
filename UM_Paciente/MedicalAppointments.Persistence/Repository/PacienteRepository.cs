using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using MedicalAppointments.Patients.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;

namespace MedicalAppointments.Patients.Persistence.Repository
{
    internal class PacienteRepository : RepositoryGeneric<Paciente>, IPaciente
    {
        private readonly AppDbContext _context;
        public PacienteRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<string> GenerateNextCodigoAsync(CancellationToken cancellationToken)
        {
            // Obtener el último código numérico
            var last = await _context.Set<Paciente>()
                .OrderByDescending(p => p.CodigoPaciente)
                .Select(p => p.CodigoPaciente)
                .FirstOrDefaultAsync(cancellationToken);

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(last))
            {
                var parts = last.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                    nextNumber = num + 1;
            }

            return $"PAC-{nextNumber:D4}"; // PAC-0001, PAC-0002, ...
        }
    }
}
