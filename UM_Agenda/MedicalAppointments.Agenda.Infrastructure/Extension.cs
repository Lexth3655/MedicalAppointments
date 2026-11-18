using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Agenda.Infrastructure;

/// <summary>Registro de servicios técnicos propios del microservicio Agenda.</summary>
public static class Extension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services;
}
