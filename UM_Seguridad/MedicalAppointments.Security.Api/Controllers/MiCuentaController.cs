using System.Security.Claims;
using MediatR;
using MedicalAppointments.Security.Core.Features.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Security.Api.Controllers;

[ApiController, Route("api/cuenta"), Authorize]
public sealed class MiCuentaController(IMediator mediator) : ControllerBase
{
    [HttpPatch("perfil")]
    public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarMiPerfilRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var error = await mediator.Send(new ActualizarMiPerfilCommand(userId, request.Nombre), ct);
        if (error is not null) return BadRequest(new { message = error });
        HttpContext.Items["AuditAction"] = "Actualizó su perfil";
        HttpContext.Items["AuditResource"] = "Usuarios";
        HttpContext.Items["AuditDetail"] = "Actualizó su nombre visible de cuenta.";
        return NoContent();
    }

    [HttpPatch("contrasena")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarMiContrasenaRequest request, CancellationToken ct)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var error = await mediator.Send(new CambiarMiContrasenaCommand(userId, request.ContrasenaActual, request.ContrasenaNueva), ct);
        if (error is not null) return BadRequest(new { message = error });
        HttpContext.Items["AuditAction"] = "Cambió su contraseña";
        HttpContext.Items["AuditResource"] = "Usuarios";
        HttpContext.Items["AuditDetail"] = "Cambió la contraseña de su cuenta.";
        return NoContent();
    }
}

public sealed record ActualizarMiPerfilRequest(string Nombre);
public sealed record CambiarMiContrasenaRequest(string ContrasenaActual, string ContrasenaNueva);
