using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Persistence.Data;
using MedicalAppointments.ClinicalCare.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.ClinicalCare.Persistence;

public static class Extension
{
    public static IServiceCollection AddClinicalCarePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ClinicalCare")
            ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:ClinicalCare.");
        services.AddDbContext<ClinicalCareDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IClinicalCareRepository, ClinicalCareRepository>();
        return services;
    }
}
