using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using MedicalAppointments.Security.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Security.Persistence.Repositories;

internal sealed class AuditoriaRepository(SecurityDbContext context) : IAuditoriaRepository
{
    public async Task RegistrarAsync(Auditoria auditoria, CancellationToken cancellationToken)
    {
        context.Auditoria.Add(auditoria);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<Auditoria> Items, int Total)> ListarAsync(int pagina, int tamano, DateTime? desdeUtc, DateTime? hastaUtc, CancellationToken cancellationToken)
    {
        var query = context.Auditoria.AsNoTracking().AsQueryable();
        if (desdeUtc.HasValue) query = query.Where(x => x.FechaUtc >= desdeUtc.Value);
        if (hastaUtc.HasValue) query = query.Where(x => x.FechaUtc <= hastaUtc.Value);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.FechaUtc)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(cancellationToken);
        return (items, total);
    }
}
