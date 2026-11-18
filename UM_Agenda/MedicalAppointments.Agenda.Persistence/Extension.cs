using MedicalAppointments.Agenda.Core.Interfaces.Repositorios;
using MedicalAppointments.Agenda.Persistence.Data;
using MedicalAppointments.Agenda.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetClass.Abstractions;
using NugetClass.Implementations;

namespace MedicalAppointments.Agenda.Persistence;

public static class Extension
{
    // Se conserva esta firma porque Program.cs ya la utiliza.
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();
        return AddPersistence(services, configuration);
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["sql:cx"]
            ?? throw new InvalidOperationException("No se configuró la cadena de conexión 'sql:cx'.");

        services.AddDbContext<AgendaContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AgendaContext>());
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryGeneric<>));
        services.AddScoped<IPaginationEspecialidades, PaginationEspecialidades>();
        services.AddScoped<IPaginationConsultorios, PaginationConsultorios>();
        services.AddScoped<IPaginationMedicos, PaginationMedicos>();
        services.AddScoped<IPaginationHorariosMedicos, PaginationHorariosMedicos>();
        services.AddScoped<IPaginationCitas, PaginationCitas>();
        services.AddScoped<IMedicosRepository, MedicosRepository>();
        services.AddScoped<IHorariosMedicosRepository, HorariosMedicosRepository>();
        services.AddScoped<ICitasRepository, CitasRepository>();
        return services;
    }
}
