using MediatR;
using MedicalAppointments.Agenda.Core.Feature.Especialidad.Commands;
using MedicalAppointments.Agenda.Core.Feature.Especialidad.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController]
[Route("api/especialidades")]
public sealed class EspecialidadesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<Especialidades>> Listar([FromQuery] ListarEspecialidadesQuery query, CancellationToken ct)
        => mediator.Send(query, ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Especialidades>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerEspecialidadPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<Especialidades>> Crear(CrearEspecialidadCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, ActualizarEspecialidadCommand command, CancellationToken ct)
        => await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromQuery] bool activo, CancellationToken ct)
        => await mediator.Send(new CambiarEstadoEspecialidadCommand(id, activo), ct) ? NoContent() : NotFound();
}
