using MediatR;
using MedicalAppointments.Agenda.Core.Feature.Cita.Commands;
using MedicalAppointments.Agenda.Core.Feature.Cita.Queries;
using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NugetClass.Pagination.Models;

namespace MedicalAppointments.Agenda.Api.Controllers;

[ApiController, Authorize]
[Route("api/citas")]
public sealed class CitasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<Citas>> Listar([FromQuery] ListarCitasQuery query, CancellationToken ct)
    {
        if (User.IsInRole("Medico"))
        {
            query.MedicoId = TryGetDoctorId(out var doctorId) ? doctorId : long.MaxValue;
        }
        return mediator.Send(query, ct);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Citas>> Obtener(long id, CancellationToken ct)
    {
        var item = await mediator.Send(new ObtenerCitaPorIdQuery(id), ct);
        if (item is null || (User.IsInRole("Medico") && (!TryGetDoctorId(out var doctorId) || item.MedicoId != doctorId)))
            return NotFound();
        return Ok(item);
    }

    [HttpGet("medico/{medicoId:long}/dia/{fecha}")]
    public Task<IReadOnlyList<Citas>> AgendaDelDia(long medicoId, DateOnly fecha, CancellationToken ct)
    {
        if (User.IsInRole("Medico") && (!TryGetDoctorId(out var ownDoctorId) || medicoId != ownDoctorId))
            return Task.FromResult<IReadOnlyList<Citas>>(Array.Empty<Citas>());
        return mediator.Send(new ListarAgendaDelDiaQuery(medicoId, fecha), ct);
    }

    [HttpGet("paciente/{pacienteId:long}")]
    public async Task<IReadOnlyList<Citas>> PorPaciente(long pacienteId, CancellationToken ct)
    {
        var items = await mediator.Send(new ListarCitasPorPacienteQuery(pacienteId), ct);
        if (!User.IsInRole("Medico")) return items;
        if (!TryGetDoctorId(out var doctorId)) return Array.Empty<Citas>();
        return items.Where(item => item.MedicoId == doctorId).ToArray();
    }

    [HttpPost, Authorize(Roles = "Recepcion,Administrador")]
    public async Task<ActionResult<Citas>> Crear(CrearCitaCommand command, CancellationToken ct)
    {
        var created = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Obtener), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}"), Authorize(Roles = "Medico,Administrador")]
    public async Task<IActionResult> Actualizar(long id, ActualizarCitaCommand command, CancellationToken ct)
    {
        if (User.IsInRole("Medico"))
        {
            var current = await mediator.Send(new ObtenerCitaPorIdQuery(id), ct);
            if (current is null) return NotFound();
            if (!TryGetDoctorId(out var doctorId) || current.MedicoId != doctorId ||
                command.MedicoId != doctorId || command.PacienteId != current.PacienteId)
                return Forbid();
            command = command with { UsuarioReserva = current.UsuarioReserva };
        }
        return await mediator.Send(command with { Id = id }, ct) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/estado"), Authorize(Roles = "Medico,Administrador")]
    public async Task<IActionResult> CambiarEstado(long id, [FromBody] string estado, CancellationToken ct)
    {
        if (User.IsInRole("Medico"))
        {
            var current = await mediator.Send(new ObtenerCitaPorIdQuery(id), ct);
            if (current is null || !TryGetDoctorId(out var doctorId) || current.MedicoId != doctorId) return NotFound();
        }
        return await mediator.Send(new CambiarEstadoCitaCommand(id, estado), ct) ? NoContent() : NotFound();
    }

    [HttpPatch("{id:long}/cancelar"), Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Cancelar(long id, CancellationToken ct)
        => await mediator.Send(new CancelarCitaCommand(id), ct) ? NoContent() : NotFound();

    private bool TryGetDoctorId(out long doctorId) =>
        long.TryParse(User.FindFirstValue("medicoId"), out doctorId);
}
