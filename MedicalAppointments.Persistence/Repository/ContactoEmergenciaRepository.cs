using MedicalAppointments.Core.Interfaces.Repositorios;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using NugetClass.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalAppointments.Persistence.Repository
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
