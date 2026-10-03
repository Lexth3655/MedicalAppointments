using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MedicalAppointments.Patients.Core.Configs;
using System.Reflection;

namespace MedicalAppointments.Patients.Core
{
    public static class Extension
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.Configure<DownstreamOptions>(configuration.GetSection("Downstream"));
            return services;
        }

    }
}
