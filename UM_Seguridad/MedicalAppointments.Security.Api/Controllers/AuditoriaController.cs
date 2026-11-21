using MediatR;
using MedicalAppointments.Security.Core.Features.Auditoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Security.Api.Controllers;

[ApiController]
[Route("api/auditoria")]
[Authorize(Roles = "Administrador")]
public sealed class AuditoriaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ListarAuditoriaResult>> Listar([FromQuery] int pagina = 1, [FromQuery] int tamano = 50,
        [FromQuery] DateTime? desdeUtc = null, [FromQuery] DateTime? hastaUtc = null, CancellationToken cancellationToken = default)
    {
        if (pagina < 1 || tamano is < 1 or > 200 || (desdeUtc.HasValue && hastaUtc.HasValue && desdeUtc > hastaUtc))
            return BadRequest(new { message = "Paginación o rango de fechas inválido." });
        return Ok(await mediator.Send(new ListarAuditoriaQuery(pagina, tamano, desdeUtc, hastaUtc), cancellationToken));
    }
}
