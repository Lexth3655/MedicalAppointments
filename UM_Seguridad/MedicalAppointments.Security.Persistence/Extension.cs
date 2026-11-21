using MedicalAppointments.Security.Core.Contracts;
using MedicalAppointments.Security.Persistence.Data;
using MedicalAppointments.Security.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Security.Persistence;

public static class Extension
{
    public static IServiceCollection AddSecurityPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Security")
            ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:Security.");
        services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<INotificacionRepository, NotificacionRepository>();
        return services;
    }
}
