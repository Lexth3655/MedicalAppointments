using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record CambiarEstadoUsuarioCommand(Guid Id, bool Activo, Guid UsuarioSolicitante) : IRequest<string?>;

public sealed class CambiarEstadoUsuarioCommandHandler(IUsuarioRepository usuarios)
    : IRequestHandler<CambiarEstadoUsuarioCommand, string?>
{
    public async Task<string?> Handle(CambiarEstadoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(request.Id, cancellationToken);
        if (usuario is null) return "NoEncontrado";
        if (!request.Activo && request.UsuarioSolicitante == request.Id) return "NoPuedeDesactivarse";
        usuario.Activo = request.Activo;
        await usuarios.GuardarCambiosAsync(cancellationToken);
        return null;
    }
}
