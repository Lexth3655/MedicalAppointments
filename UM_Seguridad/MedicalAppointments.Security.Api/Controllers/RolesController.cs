using MediatR;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Core.Features.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Security.Api.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public sealed class RolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<RolResponse>> Listar(CancellationToken cancellationToken) =>
        mediator.Send(new ListarRolesQuery(), cancellationToken);

    [HttpPost, Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear(GuardarRolRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CrearRolCommand(request), cancellationToken);
        if (result.Rol is null) return result.Conflicto ? Conflict(new { message = result.Error }) : BadRequest(new { message = result.Error });
        HttpContext.Items["AuditAction"] = "Creó un rol";
        HttpContext.Items["AuditResource"] = "Roles";
        HttpContext.Items["AuditDetail"] = $"Creó el rol '{result.Rol.Nombre}'. Módulos habilitados: {DescribePermissions(result.Rol.Permisos)}.";
        return StatusCode(StatusCodes.Status201Created, result.Rol);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, GuardarRolRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ActualizarRolCommand(id, request), cancellationToken);
        if (result.Rol is null) return result.Conflicto ? Conflict(new { message = result.Error }) : result.Error == "No se encontró el rol." ? NotFound() : BadRequest(new { message = result.Error });
        HttpContext.Items["AuditAction"] = "Actualizó permisos del rol";
        HttpContext.Items["AuditResource"] = "Roles";
        HttpContext.Items["AuditDetail"] = $"Actualizó el rol '{result.Rol.Nombre}'. Módulos habilitados: {DescribePermissions(result.Rol.Permisos)}.";
        return Ok(result.Rol);
    }

    private static string DescribePermissions(IReadOnlyList<PermisoRolResponse> permissions)
    {
        var names = new Dictionary<string,string> { ["dashboard"]="Inicio", ["patients"]="Pacientes", ["patients-emergency-contacts"]="Pacientes · Contactos de emergencia", ["agenda-appointments"]="Agenda · Citas", ["agenda-doctors"]="Agenda · Médicos", ["agenda-specialties"]="Agenda · Especialidades", ["agenda-clinics"]="Agenda · Consultorios", ["agenda-schedules"]="Agenda · Horarios", ["clinical-attentions"]="Clínica · Atenciones", ["clinical-vitals"]="Clínica · Signos vitales", ["clinical-notes"]="Clínica · Notas médicas", ["clinical-medicines"]="Clínica · Medicamentos", ["clinical-prescriptions"]="Clínica · Prescripciones", ["security-users"]="Seguridad · Usuarios", ["security-roles"]="Seguridad · Roles", ["security-audit"]="Seguridad · Auditoría" };
        return string.Join("; ", permissions.Where(p => p.Leer || p.Crear || p.Editar || p.Eliminar)
            .Select(p => $"{names.GetValueOrDefault(p.Modulo, p.Modulo)}: {string.Join(", ", new[] { p.Leer ? "consultar" : null, p.Crear ? "crear" : null, p.Editar ? "editar" : null, p.Eliminar ? "eliminar" : null }.Where(x => x is not null))}"));
    }
}
