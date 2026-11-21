using MediatR;
using MedicalAppointments.ClinicalCare.Core.Features.Atenciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAppointments.ClinicalCare.Api.Controllers;

[ApiController, Route("api/atenciones")]
public sealed class AtencionesController(IMediator mediator) : ControllerBase
{
    [HttpGet] public Task<List<MedicalAppointments.ClinicalCare.Domain.Entities.Atencion>> Listar([FromQuery] long? pacienteId, CancellationToken ct) => mediator.Send(new ListarAtencionesQuery(pacienteId), ct);
    [HttpGet("{id:long}")] public async Task<IActionResult> Obtener(long id, CancellationToken ct) => await mediator.Send(new ObtenerAtencionQuery(id), ct) is { } item ? Ok(item) : NotFound();
    [HttpPost, Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Crear(CrearAtencionCommand command, CancellationToken ct) { var result = await mediator.Send(command, ct); return result.Atencion is null ? (result.Conflicto ? Conflict(new { message = result.Error }) : BadRequest(new { message = result.Error })) : CreatedAtAction(nameof(Obtener), new { id = result.Atencion.Id }, result.Atencion); }
    [HttpPut("{id:long}"), Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Actualizar(long id, ActualizarAtencionCommand command, CancellationToken ct) => await mediator.Send(command with { Id = id }, ct) is { } item ? Ok(item) : NotFound();
    [HttpPatch("{id:long}/estado"), Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Estado(long id, CambiarEstadoAtencionCommand command, CancellationToken ct) => await mediator.Send(command with { Id = id }, ct) is { } item ? Ok(item) : BadRequest();
}
