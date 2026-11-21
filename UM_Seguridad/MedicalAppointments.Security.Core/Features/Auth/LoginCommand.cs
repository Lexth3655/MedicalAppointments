using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Auth;

public sealed record LoginCommand(string Username, string Password) : IRequest<LoginResponse?>;

public sealed class LoginCommandHandler(IUsuarioRepository usuarios, IPasswordService passwords, ITokenService tokens)
    : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var usuario = await usuarios.ObtenerPorUsernameAsync(username, cancellationToken);
        if (usuario is null || !usuario.Activo || !passwords.Verificar(usuario, request.Password)) return null;
        var (token, expiraUtc) = tokens.CrearToken(usuario);
        return new LoginResponse(token, expiraUtc, usuario.Username, usuario.Nombre, usuario.Rol.Nombre, usuario.MedicoId);
    }
}
