using MedicalAppointments.Patients.Core;
using MedicalAppointments.Patients.Core.Services;
using MedicalAppointments.Patients.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetPackage_Rest.Interfaces.IServices;

namespace MedicalAppointments.Tests;

public class InfrastructureRegistrationTests
{
    [Fact]
    public void AddCoreAndInfrastructure_RegisterRestClientAndCitasService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RestSettings:EnableRequestLogs"] = "false",
                ["Downstream:CitasBaseUrl"] = "http://citas.local"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCore(configuration);
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IRest>());
        Assert.NotNull(provider.GetRequiredService<ICitasService>());
    }
}
