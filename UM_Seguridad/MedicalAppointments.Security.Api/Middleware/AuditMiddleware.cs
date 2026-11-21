using System.Diagnostics;
using System.Security.Claims;
using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Domain.Entities;

namespace MedicalAppointments.Security.Api.Middleware;

public sealed class AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IAuditoriaRepository auditoria)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            await next(context);
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            throw;
        }
        finally
        {
            try
            {
                var idClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var auditRow = new Auditoria
                {
                    FechaUtc = DateTime.UtcNow,
                    Metodo = context.Request.Method,
                    Ruta = context.Request.Path.Value ?? "/",
                    Recurso = context.Items["AuditResource"] as string ?? GetResource(context.Request.Path),
                    Accion = context.Items["AuditAction"] as string ?? GetAction(context.Request.Method),
                    Detalle = Limit(context.Items["AuditDetail"] as string ?? $"{context.Request.Method} {context.Request.Path} respondió {context.Response.StatusCode}.", 1000),
                    CodigoEstado = context.Response.StatusCode,
                    UsuarioId = Guid.TryParse(idClaim, out var userId) ? userId : null,
                    Username = context.User.Identity?.Name,
                    DireccionIp = context.Connection.RemoteIpAddress?.ToString(),
                    AgenteUsuario = context.Request.Headers.UserAgent.ToString() is { Length: > 512 } agent ? agent[..512] : context.Request.Headers.UserAgent.ToString(),
                    TraceId = context.TraceIdentifier,
                    DuracionMs = (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds
                };
                await auditoria.RegistrarAsync(auditRow, CancellationToken.None);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "No se pudo guardar la auditoría de la petición {TraceId}", context.TraceIdentifier);
            }
        }
    }

    private static string GetResource(PathString path)
    {
        var parts = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
        if (parts.Length < 2) return "Sistema";
        return parts[1].ToLowerInvariant() switch
        {
            "roles" => "Roles", "usuarios" => "Usuarios", "auditoria" => "Auditoría", "auth" => "Autenticación",
            var name => char.ToUpperInvariant(name[0]) + name[1..]
        };
    }
    private static string GetAction(string method) => method.ToUpperInvariant() switch
    {
        "POST" => "Creó o envió datos", "PUT" or "PATCH" => "Actualizó datos", "DELETE" => "Eliminó datos", _ => "Consultó información"
    };
    private static string Limit(string value, int max) => value.Length <= max ? value : value[..max];
}
