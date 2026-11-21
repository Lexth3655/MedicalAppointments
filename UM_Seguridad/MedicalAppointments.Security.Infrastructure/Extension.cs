using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MedicalAppointments.Security.Infrastructure;

public static class Extension
{
    public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName)).Validate(x =>
            !string.IsNullOrWhiteSpace(x.Issuer) && !string.IsNullOrWhiteSpace(x.Audience) &&
            !string.IsNullOrWhiteSpace(x.Key) && System.Text.Encoding.UTF8.GetByteCount(x.Key) >= 32 && x.ExpirationMinutes > 0,
            "La configuración Jwt requiere Issuer, Audience, una Key de al menos 32 bytes y ExpirationMinutes válido.").ValidateOnStart();
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        return services;
    }
}
