using Microsoft.Extensions.Options;

namespace MedicalAppointments.Security.Infrastructure.Services;

internal sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options) =>
        !string.IsNullOrWhiteSpace(options.Issuer) && !string.IsNullOrWhiteSpace(options.Audience) &&
        !string.IsNullOrWhiteSpace(options.Key) && System.Text.Encoding.UTF8.GetByteCount(options.Key) >= 32 && options.ExpirationMinutes > 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("La configuración Jwt requiere Issuer, Audience, una Key de al menos 32 bytes y ExpirationMinutes válido.");
}
