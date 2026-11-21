using MediatR;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Core.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Security.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(request.Username, request.Password), cancellationToken);
        return response is null ? Unauthorized(new { message = "Usuario o contraseña inválidos." }) : Ok(response);
    }
}
