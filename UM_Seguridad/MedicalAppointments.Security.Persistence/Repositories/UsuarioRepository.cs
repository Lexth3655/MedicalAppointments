using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;
using MedicalAppointments.Security.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Security.Persistence.Repositories;

internal sealed class UsuarioRepository(SecurityDbContext context) : IUsuarioRepository
{
    public Task<Usuario?> ObtenerPorUsernameAsync(string username, CancellationToken cancellationToken) =>
        context.Usuarios.Include(x => x.Rol).ThenInclude(x => x.Permisos).SingleOrDefaultAsync(x => x.Username == username, cancellationToken);

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        context.Usuarios.Include(x => x.Rol).ThenInclude(x => x.Permisos).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<List<Usuario>> ListarAsync(CancellationToken cancellationToken) =>
        context.Usuarios.Include(x => x.Rol).OrderBy(x => x.Username).ToListAsync(cancellationToken);

    public async Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken) =>
        await context.Usuarios.AddAsync(usuario, cancellationToken);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);

    public Task<bool> RolExisteAsync(int rolId, CancellationToken cancellationToken) =>
        context.Roles.AnyAsync(x => x.Id == rolId, cancellationToken);

    public Task<bool> UsernameExisteAsync(string username, CancellationToken cancellationToken, Guid? excluirUsuarioId = null) =>
        context.Usuarios.AnyAsync(x => x.Username == username && (!excluirUsuarioId.HasValue || x.Id != excluirUsuarioId.Value), cancellationToken);
}
