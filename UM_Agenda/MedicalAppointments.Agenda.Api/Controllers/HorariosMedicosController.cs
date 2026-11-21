using MediatR;
using MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;
using MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using NugetClass.Pagination.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController, Authorize]
[Route("api/horarios-medicos")]
public sealed class HorariosMedicosController(IMediator mediator) : ControllerBase
{
    [HttpGet("medico/{medicoId:long}")]
    public async Task<ActionResult<PagedResult<HorariosMedicos>>> ListarPorMedico(long medicoId,
        [FromQuery] ListarHorariosPorMedicoQuery query, CancellationToken ct)
    {
        if (User.IsInRole("Medico") && User.FindFirstValue("medicoId") != medicoId.ToString())
            return Forbid();
        query.MedicoId = medicoId;
        return Ok(await mediator.Send(query, ct));
    }

    [HttpGet("medico/{medicoId:long}/disponibilidad/{fecha}")]
    public Task<IReadOnlyList<TimeOnly>> ObtenerDisponibilidad(long medicoId, DateOnly fecha, CancellationToken ct)
        => mediator.Send(new ObtenerDisponibilidadQuery(medicoId, fecha), ct);

    [HttpGet("{id:long}")]
    public async Task<ActionResult<HorariosMedicos>> Obtener(long id, CancellationToken ct)
        => await mediator.Send(new ObtenerHorarioPorIdQuery(id), ct) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    [Authorize(Roles = "Administrador,SupervisorTecnico,Medico")]
    public async Task<ActionResult<HorariosMedicos>> Crear(CrearHorarioCommand command, CancellationToken ct)
    {
        if (User.IsInRole("Medico") && User.FindFirstValue("medicoId") != command.MedicoId.ToString()) return Forbid();
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Administrador,SupervisorTecnico,Medico")]
    public async Task<IActionResult> Actualizar(long id, ActualizarHorarioCommand command, CancellationToken ct)
    {
        if (User.IsInRole("Medico"))
        {
            var item = await mediator.Send(new ObtenerHorarioPorIdQuery(id), ct);
            if (item is null) return NotFound();
            if (User.FindFirstValue("medicoId") != item.MedicoId.ToString()) return Forbid();
        }
        return await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/estado")]
    [Authorize(Roles = "Administrador,SupervisorTecnico,Medico")]
    public async Task<IActionResult> CambiarEstado(long id, [FromQuery] bool activo, CancellationToken ct)
    {
        if (User.IsInRole("Medico"))
        {
            var item = await mediator.Send(new ObtenerHorarioPorIdQuery(id), ct);
            if (item is null) return NotFound();
            if (User.FindFirstValue("medicoId") != item.MedicoId.ToString()) return Forbid();
        }
        return await mediator.Send(new CambiarEstadoHorarioCommand(id, activo), ct) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/estado-turno"), Authorize(Roles = "Administrador,SupervisorTecnico,Medico")]
    public async Task<IActionResult> ActualizarEstadoTurno(long id, [FromBody] EstadoTurnoRequest request, CancellationToken ct)
    {
        var item = await mediator.Send(new ObtenerHorarioPorIdQuery(id), ct);
        if (item is null) return NotFound();
        if (User.IsInRole("Medico") && User.FindFirstValue("medicoId") != item.MedicoId.ToString()) return Forbid();
        return await mediator.Send(new ActualizarEstadoTurnoCommand(id, request.EstadoTurno), ct) ? NoContent() : BadRequest("Estado de turno inválido.");
    }
}

public sealed record EstadoTurnoRequest(string EstadoTurno);
