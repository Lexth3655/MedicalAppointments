using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Usuarios;

public sealed record ListarUsuariosQuery : IRequest<IReadOnlyList<UsuarioResponse>>;

public sealed class ListarUsuariosQueryHandler(IUsuarioRepository usuarios)
    : IRequestHandler<ListarUsuariosQuery, IReadOnlyList<UsuarioResponse>>
{
    public async Task<IReadOnlyList<UsuarioResponse>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken) =>
        (await usuarios.ListarAsync(cancellationToken)).Select(usuario => new UsuarioResponse(usuario.Id, usuario.Username,
            usuario.Nombre, usuario.RolId, usuario.Rol.Nombre, usuario.MedicoId, usuario.Activo, usuario.FechaCreacionUtc)).ToList();
}
