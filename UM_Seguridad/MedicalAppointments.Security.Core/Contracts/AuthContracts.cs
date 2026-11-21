namespace MedicalAppointments.Security.Core.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string Username, string Nombre, string Rol, long? MedicoId);
public sealed record CrearUsuarioRequest(string Username, string Password, string Nombre, int RolId, long? MedicoId);
public sealed record ActualizarUsuarioRequest(string Username, string? Password, string Nombre, int RolId, long? MedicoId);
public sealed record UsuarioResponse(Guid Id, string Username, string Nombre, int RolId, string Rol, long? MedicoId, bool Activo, DateTime FechaCreacionUtc);
public sealed record PermisoRolResponse(string Modulo, bool Leer, bool Crear, bool Editar, bool Eliminar);
public sealed record RolResponse(int Id, string Nombre, IReadOnlyList<PermisoRolResponse> Permisos);
public sealed record GuardarRolRequest(string Nombre, IReadOnlyList<PermisoRolResponse> Permisos);
public sealed record AuditoriaResponse(long Id, DateTime FechaUtc, string Metodo, string Ruta, string Recurso, string Accion, string? Detalle, int CodigoEstado, Guid? UsuarioId, string? Username, string? DireccionIp, string? AgenteUsuario, string? TraceId, long DuracionMs);
