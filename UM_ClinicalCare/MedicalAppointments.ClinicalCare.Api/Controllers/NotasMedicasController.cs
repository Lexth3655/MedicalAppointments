using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Core.Features.NotasMedicas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MedicalAppointments.ClinicalCare.Api.Controllers;

[ApiController, Authorize]
public sealed class NotasMedicasController(IMediator mediator) : ControllerBase
{
    [HttpGet("api/atenciones/{atencionId:long}/notas")] public Task<List<MedicalAppointments.ClinicalCare.Domain.Entities.NotaMedica>> Listar(long atencionId, CancellationToken ct) => mediator.Send(new ListarNotasQuery(atencionId), ct);
    [HttpPost("api/atenciones/{atencionId:long}/notas"), Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Crear(long atencionId, CrearNotaMedicaRequest request, CancellationToken ct)
    {
        long? medicoId = long.TryParse(User.FindFirstValue("medicoId"), out var parsedId) ? parsedId : null;
        var createdBy = User.Identity?.Name;
        var item = await mediator.Send(new CrearNotaCommand(atencionId, request.Nota, medicoId, createdBy), ct);
        return item is null ? BadRequest() : Created($"/api/notas-medicas/{item.Id}", item);
    }
    [HttpPut("api/notas-medicas/{id:long}"), Authorize(Roles = "Medico,Administrador")] public async Task<IActionResult> Actualizar(long id, ActualizarNotaMedicaRequest request, CancellationToken ct) => await mediator.Send(new ActualizarNotaCommand(id, request.Nota), ct) is { } item ? Ok(item) : NotFound();
}
