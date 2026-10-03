using MediatR;
using MedicalAppointments.Patients.Core.Exceptions;
using MedicalAppointments.Patients.Core.Feature.Appointments.Queries;
using MedicalAppointments.Patients.Core.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Patients.Api.Controllers;

[ApiController]
[Route("api/pacientes")]
public sealed class CitasController(IMediator mediator) : ControllerBase
{
    [HttpGet("{pacienteId:long}/citas")]
    [ProducesResponseType(typeof(IReadOnlyList<CitaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IReadOnlyList<CitaDto>>> GetByPatient(long pacienteId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new GetCitasPacienteQuery(pacienteId), cancellationToken));
        }
        catch (DownstreamServiceException ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Servicio de Citas no disponible",
                Detail = ex.Message,
                Status = StatusCodes.Status503ServiceUnavailable
            };
            problem.Extensions["errorCode"] = ex.ErrorCode;
            return StatusCode(StatusCodes.Status503ServiceUnavailable, problem);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(title: "Configuración incompleta", detail: ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }
}
