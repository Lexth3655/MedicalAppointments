using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Patients.Infrastructure
{
    public static class Extension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (ServiceProvider provider = services.BuildServiceProvider())
                configuration = provider.GetRequiredService<IConfiguration>();
            // Aquí se registrarían HttpClients tipados hacia otros microservicios
            // ej: services.AddHttpClient<IPatientServiceClient, PatientServiceClient>(...)

            return services;
        }
    }
}
