using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.ClinicalCare.Infrastructure;

public static class Extension
{
    public static IServiceCollection AddClinicalCareInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IClinicalCareExternalValidator, AgendaClinicalCareValidator>(client =>
            client.BaseAddress = new Uri(configuration["Downstream:AgendaBaseUrl"] ?? "http://localhost:5260/"));
        return services;
    }
}
