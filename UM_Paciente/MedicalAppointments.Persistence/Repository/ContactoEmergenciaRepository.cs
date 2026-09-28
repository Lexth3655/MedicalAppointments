using MedicalAppointments.Patients.Core.Interfaces.Repositorios;
using MedicalAppointments.Patients.Domain.Models;
using MedicalAppointments.Patients.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;

namespace MedicalAppointments.Patients.Persistence.Repository
{
    internal class ContactoEmergenciaRepository : RepositoryGeneric<ContactoEmergencia>, IContactoEmergencia
    {
        private readonly AppDbContext _context;
        public ContactoEmergenciaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ContactoEmergencia>> GetByPacienteIdAsync(long pacienteId, CancellationToken cancellationToken = default)
        {
            var list = await _context.ContactosEmergencia
                                     .AsNoTracking()
                                     .Where(x => x.PacienteId == pacienteId)
                                     .OrderBy(x => x.Prioridad)
                                     .ToListAsync(cancellationToken);
            return list; // List<ContactoEmergencia> implementa IReadOnlyList<ContactoEmergencia>
        }
    }
}
