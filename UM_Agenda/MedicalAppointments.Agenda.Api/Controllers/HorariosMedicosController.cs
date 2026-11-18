using MediatR;
using MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;
using MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController]
[Route("api/horarios-medicos")]
public sealed class HorariosMedicosController(IMediator mediator) : ControllerBase
{
    [HttpGet("medico/{medicoId:long}")]
    public Task<PagedResult<HorariosMedicos>> ListarPorMedico(long medicoId,
        [FromQuery] ListarHorariosPorMedicoQuery query, CancellationToken ct)
    {
        query.MedicoId = medicoId;
        return mediator.Send(query, ct);
    }

    [HttpGet("medico/{medicoId:long}/disponibilidad/{fecha}")]
    public Task<IReadOnlyList<TimeOnly>> ObtenerDisponibilidad(long medicoId, DateOnly fecha, CancellationToken ct)
        => mediator.Send(new ObtenerDisponibilidadQuery(medicoId, fecha), ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<HorariosMedicos>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerHorarioPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<HorariosMedicos>> Crear(CrearHorarioCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, ActualizarHorarioCommand command, CancellationToken ct)
        => await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/estado")]
    public async Task<IActionResult> CambiarEstado(long id, [FromQuery] bool activo, CancellationToken ct)
        => await mediator.Send(new CambiarEstadoHorarioCommand(id, activo), ct) ? NoContent() : NotFound();
}
