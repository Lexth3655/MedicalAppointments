using System.Security.Claims;
using MediatR;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Core.Features.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Security.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public sealed class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<UsuarioResponse>> Listar(CancellationToken cancellationToken) =>
        mediator.Send(new ListarUsuariosQuery(), cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioResponse>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var usuario = await mediator.Send(new ObtenerUsuarioQuery(id), cancellationToken);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Crear(CrearUsuarioRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CrearUsuarioCommand(request.Username, request.Password, request.Nombre,
            request.RolId, request.MedicoId), cancellationToken);
        if (result.Error is not null)
            return result.Conflicto ? Conflict(new { message = result.Error }) : BadRequest(new { message = result.Error });
        HttpContext.Items["AuditAction"] = "Creó un usuario";
        HttpContext.Items["AuditResource"] = "Usuarios";
        HttpContext.Items["AuditDetail"] = $"Creó la cuenta '{result.Usuario!.Username}' para {result.Usuario.Nombre} con rol '{result.Usuario.Rol}'.";
        return CreatedAtAction(nameof(Obtener), new { id = result.Usuario.Id }, result.Usuario);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, ActualizarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ActualizarUsuarioCommand(id, request.Username, request.Password,
            request.Nombre, request.RolId, request.MedicoId), cancellationToken);
        if (result.Error is not null)
            return result.Conflicto ? Conflict(new { message = result.Error }) : result.Error == "No se encontró la cuenta." ? NotFound() : BadRequest(new { message = result.Error });
        HttpContext.Items["AuditAction"] = "Actualizó un usuario";
        HttpContext.Items["AuditResource"] = "Usuarios";
        HttpContext.Items["AuditDetail"] = $"Actualizó la cuenta '{result.Usuario!.Username}' (ID {id}), rol '{result.Usuario.Rol}' y médico vinculado {result.Usuario.MedicoId?.ToString() ?? "ninguno"}.";
        return Ok(result.Usuario);
    }

    [HttpPatch("{id:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoUsuarioRequest request, CancellationToken cancellationToken)
    {
        var solicitante = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) ? usuarioId : Guid.Empty;
        var result = await mediator.Send(new CambiarEstadoUsuarioCommand(id, request.Activo, solicitante), cancellationToken);
        if (result is null)
        {
            HttpContext.Items["AuditAction"] = request.Activo ? "Activó un usuario" : "Desactivó un usuario";
            HttpContext.Items["AuditResource"] = "Usuarios";
            HttpContext.Items["AuditDetail"] = $"Cambió el estado de la cuenta {id} a {(request.Activo ? "activa" : "inactiva")}.";
        }
        return result switch
        {
            "NoEncontrado" => NotFound(),
            "NoPuedeDesactivarse" => BadRequest(new { message = "No puede desactivar su propia cuenta." }),
            _ => NoContent()
        };
    }

}

public sealed record CambiarEstadoUsuarioRequest(bool Activo);
