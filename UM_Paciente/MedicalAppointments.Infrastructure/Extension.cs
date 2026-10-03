using MedicalAppointments.Patients.Core.Services;
using MedicalAppointments.Patients.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;

namespace MedicalAppointments.Patients.Infrastructure;

public static class Extension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRequestLogging(configuration);
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddScoped<ICitasService, CitasService>();
        return services;
    }
}
