using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Roles;

public sealed record ListarRolesQuery : IRequest<IReadOnlyList<RolResponse>>;

public sealed class ListarRolesQueryHandler(IRolRepository roles) : IRequestHandler<ListarRolesQuery, IReadOnlyList<RolResponse>>
{
    public async Task<IReadOnlyList<RolResponse>> Handle(ListarRolesQuery request, CancellationToken cancellationToken) =>
        (await roles.ListarAsync(cancellationToken)).Select(rol => new RolResponse(rol.Id, rol.Nombre,
            rol.Permisos.OrderBy(p => p.Modulo).Select(p => new PermisoRolResponse(p.Modulo, p.Leer, p.Crear, p.Editar, p.Eliminar)).ToList())).ToList();
}
