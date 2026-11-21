using MedicalAppointments.Security.Domain.Entities;

namespace MedicalAppointments.Security.Core.Contracts;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorUsernameAsync(string username, CancellationToken cancellationToken);
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Usuario>> ListarAsync(CancellationToken cancellationToken);
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
    Task<bool> RolExisteAsync(int rolId, CancellationToken cancellationToken);
    Task<bool> UsernameExisteAsync(string username, CancellationToken cancellationToken, Guid? excluirUsuarioId = null);
}

public interface IAuditoriaRepository
{
    Task RegistrarAsync(Auditoria auditoria, CancellationToken cancellationToken);
    Task<(List<Auditoria> Items, int Total)> ListarAsync(int pagina, int tamano, DateTime? desdeUtc, DateTime? hastaUtc, CancellationToken cancellationToken);
}

public interface IRolRepository
{
    Task<List<Rol>> ListarAsync(CancellationToken cancellationToken);
    Task<Rol?> ObtenerAsync(int id, CancellationToken cancellationToken);
    Task<bool> NombreExisteAsync(string nombre, int? excluirId, CancellationToken cancellationToken);
    Task AgregarAsync(Rol rol, CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}

public interface INotificacionRepository
{
    Task<List<Usuario>> ListarRecepcionistasAsync(CancellationToken cancellationToken);
    Task AgregarAsync(IEnumerable<MedicalAppointments.Security.Domain.Entities.Notificacion> notificaciones, CancellationToken cancellationToken);
    Task<List<MedicalAppointments.Security.Domain.Entities.Notificacion>> ListarParaUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
    Task<bool> MarcarLeidaAsync(long id, Guid usuarioId, CancellationToken cancellationToken);
}

public interface IPasswordService
{
    string Hash(Usuario usuario, string password);
    bool Verificar(Usuario usuario, string password);
}

public interface ITokenService
{
    (string Token, DateTime ExpiraUtc) CrearToken(Usuario usuario);
}
