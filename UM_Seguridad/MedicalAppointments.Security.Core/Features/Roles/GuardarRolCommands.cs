using MediatR;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;

namespace MedicalAppointments.Security.Core.Features.Roles;

public sealed record CrearRolCommand(GuardarRolRequest Request) : IRequest<RoleOperationResult>;
public sealed record ActualizarRolCommand(int Id, GuardarRolRequest Request) : IRequest<RoleOperationResult>;
public sealed record RoleOperationResult(RolResponse? Rol, string? Error, bool Conflicto = false);

public sealed class CrearRolHandler(IRolRepository roles) : IRequestHandler<CrearRolCommand, RoleOperationResult>
{
    public async Task<RoleOperationResult> Handle(CrearRolCommand request, CancellationToken ct)
    {
        var validation = Validate(request.Request);
        if (validation is not null) return new(null, validation);
        var name = request.Request.Nombre.Trim();
        if (await roles.NombreExisteAsync(name, null, ct)) return new(null, "Ya existe un rol con ese nombre.", true);
        var role = new Rol { Nombre = name, Permisos = request.Request.Permisos.Select(ToEntity).ToList() };
        await roles.AgregarAsync(role, ct); await roles.GuardarCambiosAsync(ct);
        return new(ToResponse(role), null);
    }
    internal static string? Validate(GuardarRolRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) || request.Nombre.Trim().Length > 40) return "El nombre del rol es obligatorio y admite hasta 40 caracteres.";
        var known = new HashSet<string>(["dashboard", "patients", "patients-emergency-contacts", "agenda-appointments", "agenda-doctors", "agenda-specialties", "agenda-clinics", "agenda-schedules", "clinical-attentions", "clinical-vitals", "clinical-notes", "clinical-medicines", "clinical-prescriptions", "security-users", "security-roles", "security-audit"], StringComparer.OrdinalIgnoreCase);
        if (request.Permisos is null || request.Permisos.Count == 0 || request.Permisos.Any(x => x is null || !known.Contains(x.Modulo))) return "Selecciona permisos válidos para los módulos disponibles.";
        if (request.Permisos.Select(x => x.Modulo).Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.Permisos.Count) return "No repitas módulos en los permisos.";
        if (request.Permisos.Any(x => (x.Crear || x.Editar || x.Eliminar) && !x.Leer)) return "Para crear, editar o eliminar datos también se requiere permiso de lectura en ese módulo.";
        return null;
    }
    internal static PermisoRol ToEntity(PermisoRolResponse p) => new() { Modulo=p.Modulo.ToLowerInvariant(), Leer=p.Leer, Crear=p.Crear, Editar=p.Editar, Eliminar=p.Eliminar };
    internal static RolResponse ToResponse(Rol r) => new(r.Id,r.Nombre,r.Permisos.OrderBy(p=>p.Modulo).Select(p=>new PermisoRolResponse(p.Modulo,p.Leer,p.Crear,p.Editar,p.Eliminar)).ToList());
}

public sealed class ActualizarRolHandler(IRolRepository roles) : IRequestHandler<ActualizarRolCommand, RoleOperationResult>
{
    public async Task<RoleOperationResult> Handle(ActualizarRolCommand request, CancellationToken ct)
    {
        var validation = CrearRolHandler.Validate(request.Request);
        if (validation is not null) return new(null, validation);
        var role = await roles.ObtenerAsync(request.Id, ct);
        if (role is null) return new(null, "No se encontró el rol.");
        var name = request.Request.Nombre.Trim();
        if (await roles.NombreExisteAsync(name, request.Id, ct)) return new(null, "Ya existe un rol con ese nombre.", true);
        role.Nombre = name;
        role.Permisos.Clear();
        foreach (var permission in request.Request.Permisos) role.Permisos.Add(CrearRolHandler.ToEntity(permission));
        await roles.GuardarCambiosAsync(ct);
        return new(CrearRolHandler.ToResponse(role), null);
    }
}
