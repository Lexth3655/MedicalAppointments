using MediatR;
using MedicalAppointments.Agenda.Core.Feature.Consultorio.Commands;
using MedicalAppointments.Agenda.Core.Feature.Consultorio.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController]
[Route("api/consultorios")]
public sealed class ConsultoriosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<Consultorios>> Listar([FromQuery] ListarConsultoriosQuery query, CancellationToken ct)
        => mediator.Send(query, ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Consultorios>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerConsultorioPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Consultorios>> Crear(CrearConsultorioCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, ActualizarConsultorioCommand command, CancellationToken ct)
        => await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromQuery] bool activo, CancellationToken ct)
        => await mediator.Send(new CambiarEstadoConsultorioCommand(id, activo), ct) ? NoContent() : NotFound();
}
