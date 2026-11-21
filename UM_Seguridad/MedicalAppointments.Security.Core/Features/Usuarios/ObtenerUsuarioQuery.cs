using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record ObtenerUsuarioQuery(Guid Id) : IRequest<UsuarioResponse?>;

public sealed class ObtenerUsuarioQueryHandler(IUsuarioRepository usuarios)
    : IRequestHandler<ObtenerUsuarioQuery, UsuarioResponse?>
{
    public async Task<UsuarioResponse?> Handle(ObtenerUsuarioQuery request, CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(request.Id, cancellationToken);
        return usuario is null ? null : new UsuarioResponse(usuario.Id, usuario.Username, usuario.Nombre,
            usuario.RolId, usuario.Rol.Nombre, usuario.MedicoId, usuario.Activo, usuario.FechaCreacionUtc);
    }
}
