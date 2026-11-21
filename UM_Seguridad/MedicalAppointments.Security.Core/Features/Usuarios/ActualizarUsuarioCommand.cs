using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record ActualizarUsuarioCommand(Guid Id, string Username, string? Password, string Nombre, int RolId, long? MedicoId)
    : IRequest<CrearUsuarioResult>;

public sealed class ActualizarUsuarioCommandHandler(IUsuarioRepository usuarios, IPasswordService passwords)
    : IRequestHandler<ActualizarUsuarioCommand, CrearUsuarioResult>
{
    public async Task<CrearUsuarioResult> Handle(ActualizarUsuarioCommand request, CancellationToken ct)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var nombre = request.Nombre.Trim();
        if (username.Length is < 3 or > 80 || nombre.Length is < 2 or > 150 || (request.Password is { Length: > 0 and < 12 }))
            return new(null, "El usuario debe tener entre 3 y 80 caracteres, el nombre entre 2 y 150 y la contraseña nueva al menos 12 caracteres.");
        if ((request.RolId == 2) != request.MedicoId.HasValue)
            return new(null, "Solo los usuarios con rol Médico deben tener un médico vinculado.");
        var usuario = await usuarios.ObtenerPorIdAsync(request.Id, ct);
        if (usuario is null) return new(null, "No se encontró la cuenta.");
        if (!await usuarios.RolExisteAsync(request.RolId, ct)) return new(null, "El rol indicado no existe.");
        if (await usuarios.UsernameExisteAsync(username, ct, request.Id)) return new(null, "Ese nombre de usuario pertenece a otra cuenta.", true);

        usuario.Username = username;
        usuario.Nombre = nombre;
        usuario.RolId = request.RolId;
        usuario.MedicoId = request.MedicoId;
        if (!string.IsNullOrWhiteSpace(request.Password)) usuario.PasswordHash = passwords.Hash(usuario, request.Password);
        await usuarios.GuardarCambiosAsync(ct);
        var updated = await usuarios.ObtenerPorIdAsync(usuario.Id, ct);
        return new(new UsuarioResponse(updated!.Id, updated.Username, updated.Nombre, updated.RolId, updated.Rol.Nombre,
            updated.MedicoId, updated.Activo, updated.FechaCreacionUtc), null);
    }
}
