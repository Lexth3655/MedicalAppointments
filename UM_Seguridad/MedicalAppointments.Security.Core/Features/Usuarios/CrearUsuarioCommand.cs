using MediatR;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record CrearUsuarioCommand(string Username, string Password, string Nombre, int RolId, long? MedicoId)
    : IRequest<CrearUsuarioResult>;
public sealed record CrearUsuarioResult(UsuarioResponse? Usuario, string? Error, bool Conflicto = false);

public sealed class CrearUsuarioCommandHandler(IUsuarioRepository usuarios, IPasswordService passwords)
    : IRequestHandler<CrearUsuarioCommand, CrearUsuarioResult>
{
    public async Task<CrearUsuarioResult> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        if (username.Length is < 3 or > 80 || request.Password.Length < 12 || string.IsNullOrWhiteSpace(request.Nombre))
            return new(null, "El usuario debe tener entre 3 y 80 caracteres, la contraseña al menos 12 y el nombre es obligatorio.");
        if ((request.RolId == 2) != request.MedicoId.HasValue)
            return new(null, "Solo los usuarios con rol Médico deben tener MedicoId.");
        if (!await usuarios.RolExisteAsync(request.RolId, cancellationToken)) return new(null, "El rol indicado no existe.");
        if (await usuarios.UsernameExisteAsync(username, cancellationToken)) return new(null, "El nombre de usuario ya está registrado.", true);

        var usuario = new Usuario { Username = username, Nombre = request.Nombre.Trim(), RolId = request.RolId, MedicoId = request.MedicoId };
        usuario.PasswordHash = passwords.Hash(usuario, request.Password);
        await usuarios.AgregarAsync(usuario, cancellationToken);
        await usuarios.GuardarCambiosAsync(cancellationToken);
        var creado = await usuarios.ObtenerPorIdAsync(usuario.Id, cancellationToken);
        return new(new UsuarioResponse(creado!.Id, creado.Username, creado.Nombre, creado.RolId, creado.Rol.Nombre,
            creado.MedicoId, creado.Activo, creado.FechaCreacionUtc), null);
    }
}
