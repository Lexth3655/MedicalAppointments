using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using MedicalAppointments.Security.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Security.Persistence.Repositories;

internal sealed class NotificacionRepository(SecurityDbContext context) : INotificacionRepository
{
    public Task<List<Usuario>> ListarRecepcionistasAsync(CancellationToken ct) =>
        context.Usuarios.AsNoTracking().Where(x => x.RolId == 1 && x.Activo).ToListAsync(ct);
    public async Task AgregarAsync(IEnumerable<Notificacion> items, CancellationToken ct)
    { await context.Notificaciones.AddRangeAsync(items, ct); await context.SaveChangesAsync(ct); }
    public Task<List<Notificacion>> ListarParaUsuarioAsync(Guid usuarioId, CancellationToken ct) =>
        context.Notificaciones.AsNoTracking().Where(x => x.DestinatarioId == usuarioId).OrderByDescending(x => x.FechaUtc).Take(100).ToListAsync(ct);
    public async Task<bool> MarcarLeidaAsync(long id, Guid usuarioId, CancellationToken ct)
    {
        var item=await context.Notificaciones.SingleOrDefaultAsync(x=>x.Id==id&&x.DestinatarioId==usuarioId,ct);
        if(item is null)return false;item.Leida=true;await context.SaveChangesAsync(ct);return true;
    }
}
