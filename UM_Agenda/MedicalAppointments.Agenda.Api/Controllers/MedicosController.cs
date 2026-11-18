using MediatR;
using MedicalAppointments.Agenda.Core.Feature.Medico.Commands;
using MedicalAppointments.Agenda.Core.Feature.Medico.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController]
[Route("api/medicos")]
public sealed class MedicosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<Medicos>> Listar([FromQuery] ListarMedicosQuery query, CancellationToken ct)
        => mediator.Send(query, ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Medicos>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerMedicoPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Medicos>> Crear(CrearMedicoCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, ActualizarMedicoCommand command, CancellationToken ct)
        => await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromQuery] bool activo, CancellationToken ct)
        => await mediator.Send(new CambiarEstadoMedicoCommand(id, activo), ct) ? NoContent() : NotFound();
}
