using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using MedicalAppointments.Security.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Security.Persistence.Repositories;

internal sealed class RolRepository(SecurityDbContext context) : IRolRepository
{
    public Task<List<Rol>> ListarAsync(CancellationToken cancellationToken) =>
        context.Roles.AsNoTracking().Include(x => x.Permisos).OrderBy(x => x.Id).ToListAsync(cancellationToken);
    public Task<Rol?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        context.Roles.Include(x => x.Permisos).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<bool> NombreExisteAsync(string nombre, int? excluirId, CancellationToken cancellationToken) =>
        context.Roles.AnyAsync(x => x.Nombre.ToLower() == nombre.ToLower() && (!excluirId.HasValue || x.Id != excluirId.Value), cancellationToken);
    public async Task AgregarAsync(Rol rol, CancellationToken cancellationToken) => await context.Roles.AddAsync(rol, cancellationToken);
    public Task GuardarCambiosAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
