using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Core.Features.SignosVitales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MedicalAppointments.ClinicalCare.Api.Controllers;

[ApiController, Route("api/atenciones/{atencionId:long}/signos-vitales")]
public sealed class SignosVitalesController(IMediator mediator) : ControllerBase
{
    [HttpGet] public Task<List<MedicalAppointments.ClinicalCare.Domain.Entities.SignosVitales>> Listar(long atencionId, CancellationToken ct) => mediator.Send(new ListarSignosVitalesQuery(atencionId), ct);
    [HttpPut, Authorize(Roles = "Medico,Administrador,SupervisorTecnico")] public async Task<IActionResult> Guardar(long atencionId, GuardarSignosVitalesRequest request, CancellationToken ct) => await mediator.Send(new GuardarSignosVitalesCommand(atencionId, request), ct) is { } item ? Ok(item) : NotFound();
}
