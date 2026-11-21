using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Core.Features.Prescripciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAppointments.ClinicalCare.Api.Controllers;

[ApiController, Route("api/prescripciones")]
public sealed class PrescripcionesController(IMediator mediator) : ControllerBase
{
    [HttpGet] public Task<List<MedicalAppointments.ClinicalCare.Domain.Entities.Prescripcion>> Listar([FromQuery] long? atencionId, [FromQuery] long? pacienteId, CancellationToken ct) => mediator.Send(new ListarPrescripcionesQuery(atencionId, pacienteId), ct);
    [HttpGet("{id:long}")] public async Task<IActionResult> Obtener(long id, CancellationToken ct) => await mediator.Send(new ObtenerPrescripcionQuery(id), ct) is { } item ? Ok(item) : NotFound();
    [HttpPost, Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Crear(CrearPrescripcionRequest request, CancellationToken ct) { var result = await mediator.Send(new CrearPrescripcionCommand(request), ct); return result.Prescripcion is null ? BadRequest(new { message = result.Error }) : CreatedAtAction(nameof(Obtener), new { id = result.Prescripcion.Id }, result.Prescripcion); }
}
