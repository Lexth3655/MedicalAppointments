using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Core.Features.Medicamentos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAppointments.ClinicalCare.Api.Controllers;

[ApiController, Route("api/medicamentos")]
public sealed class MedicamentosController(IMediator mediator) : ControllerBase
{
    [HttpGet] public Task<List<MedicalAppointments.ClinicalCare.Domain.Entities.Medicamento>> Listar([FromQuery] bool incluirInactivos = false, CancellationToken ct = default) => mediator.Send(new ListarMedicamentosQuery(incluirInactivos), ct);
    [HttpGet("{id:long}")] public async Task<IActionResult> Obtener(long id, CancellationToken ct) => await mediator.Send(new ObtenerMedicamentoQuery(id), ct) is { } item ? Ok(item) : NotFound();
    [HttpPost, Authorize(Roles = "Administrador,SupervisorTecnico")] public async Task<IActionResult> Crear(CrearMedicamentoRequest request, CancellationToken ct) => await mediator.Send(new CrearMedicamentoCommand(request), ct) is { } item ? CreatedAtAction(nameof(Obtener), new { id = item.Id }, item) : BadRequest();
    [HttpPut("{id:long}"), Authorize(Roles = "Administrador,SupervisorTecnico")] public async Task<IActionResult> Actualizar(long id, ActualizarMedicamentoRequest request, CancellationToken ct) => await mediator.Send(new ActualizarMedicamentoCommand(id, request), ct) is { } item ? Ok(item) : NotFound();
    [HttpPatch("{id:long}/estado"), Authorize(Roles = "Administrador,SupervisorTecnico")] public async Task<IActionResult> Estado(long id, CambiarEstadoMedicamentoRequest request, CancellationToken ct) => await mediator.Send(new CambiarEstadoMedicamentoCommand(id, request.Activo), ct) is { } item ? Ok(item) : NotFound();
}
