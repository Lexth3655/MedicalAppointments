using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetPackage_Rest.Configs;

namespace NugetPackage_Rest.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Enlaza RestSettings con las opciones que consume RestBuilder.</summary>
    public static IServiceCollection AddRequestLogging(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        services.Configure<RequestSettings>(configuration.GetSection("RestSettings"));
        return services;
    }
}
