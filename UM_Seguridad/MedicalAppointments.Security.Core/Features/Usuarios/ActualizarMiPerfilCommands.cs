using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record ActualizarMiPerfilCommand(Guid Id, string Nombre) : IRequest<string?>;
public sealed record CambiarMiContrasenaCommand(Guid Id, string ContrasenaActual, string ContrasenaNueva) : IRequest<string?>;

public sealed class ActualizarMiPerfilHandler(IUsuarioRepository usuarios) : IRequestHandler<ActualizarMiPerfilCommand, string?>
{
    public async Task<string?> Handle(ActualizarMiPerfilCommand request, CancellationToken ct)
    {
        var nombre = request.Nombre.Trim();
        if (nombre.Length is < 2 or > 150) return "El nombre visible debe tener entre 2 y 150 caracteres.";
        var user = await usuarios.ObtenerPorIdAsync(request.Id, ct);
        if (user is null) return "No se encontró la cuenta.";
        user.Nombre = nombre;
        await usuarios.GuardarCambiosAsync(ct);
        return null;
    }
}

public sealed class CambiarMiContrasenaHandler(IUsuarioRepository usuarios, IPasswordService passwords)
    : IRequestHandler<CambiarMiContrasenaCommand, string?>
{
    public async Task<string?> Handle(CambiarMiContrasenaCommand request, CancellationToken ct)
    {
        if (request.ContrasenaNueva.Length < 12) return "La contraseña nueva debe tener al menos 12 caracteres.";
        var user = await usuarios.ObtenerPorIdAsync(request.Id, ct);
        if (user is null) return "No se encontró la cuenta.";
        if (!passwords.Verificar(user, request.ContrasenaActual)) return "La contraseña actual no coincide.";
        user.PasswordHash = passwords.Hash(user, request.ContrasenaNueva);
        await usuarios.GuardarCambiosAsync(ct);
        return null;
    }
}
