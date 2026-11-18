using MediatR;
using MedicalAppointments.Agenda.Core.Feature.Cita.Commands;
using MedicalAppointments.Agenda.Core.Feature.Cita.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController]
[Route("api/citas")]
public sealed class CitasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<Citas>> Listar([FromQuery] ListarCitasQuery query, CancellationToken ct)
        => mediator.Send(query, ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Citas>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerCitaPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpGet("medico/{medicoId:long}/dia/{fecha}")]
    public Task<IReadOnlyList<Citas>> AgendaDelDia(long medicoId, DateOnly fecha, CancellationToken ct)
        => mediator.Send(new ListarAgendaDelDiaQuery(medicoId, fecha), ct);

    [HttpGet("paciente/{pacienteId:long}")]
    public Task<IReadOnlyList<Citas>> PorPaciente(long pacienteId, CancellationToken ct)
        => mediator.Send(new ListarCitasPorPacienteQuery(pacienteId), ct);

    [HttpPost]
    public async Task<ActionResult<Citas>> Crear(CrearCitaCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, ActualizarCitaCommand command, CancellationToken ct)
        => await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] string estado, CancellationToken ct)
        => await mediator.Send(new CambiarEstadoCitaCommand(id, estado), ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/cancelar")]
    public async Task<IActionResult> Cancelar(long id, CancellationToken ct)
        => await mediator.Send(new CancelarCitaCommand(id), ct) ? NoContent() : NotFound();
}
