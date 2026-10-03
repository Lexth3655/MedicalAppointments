using System.Net;
using System.Text.Json;
using MedicalAppointments.Patients.Core.Configs;
using MedicalAppointments.Patients.Core.Exceptions;
using MedicalAppointments.Patients.Core.Wrappers;
using MedicalAppointments.Patients.Infrastructure.Services;
using MedicalAppointments.Tests.Fakes;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;

namespace MedicalAppointments.Tests;

public class CitasServiceTests
{
    [Fact]
    public async Task ObtenerPorPacienteAsync_WhenResponseIsSuccessful_ReturnsAppointmentsAndBuildsExpectedGet()
    {
        HttpRequestMessage? capturedRequest = null;
        var payload = new DownstreamResponse<List<CitaDto>>
        {
            Succeeded = true,
            Result = [new CitaDto { CitaId = 25, PacienteId = 7, Especialidad = "Cardiología", Estado = "Programada" }]
        };
        using var client = CreateClient(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload))
            };
        });
        var service = CreateService(client, "http://citas.local");

        var citas = await service.ObtenerPorPacienteAsync(7);

        var cita = Assert.Single(citas);
        Assert.Equal(25, cita.CitaId);
        Assert.Equal(7, cita.PacienteId);
        Assert.Equal("Cardiología", cita.Especialidad);
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest!.Method);
        Assert.Equal("http://citas.local/api/Citas/paciente/7", capturedRequest.RequestUri!.ToString());
        Assert.Null(capturedRequest.Headers.Authorization);
    }

    [Fact]
    public async Task ObtenerPorPacienteAsync_WhenRemoteServiceReturnsHttpError_TranslatesToDownstreamException()
    {
        using var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("service unavailable")
        });
        var service = CreateService(client, "http://citas.local");

        var exception = await Assert.ThrowsAsync<DownstreamServiceException>(() => service.ObtenerPorPacienteAsync(7));

        Assert.Equal(701, exception.ErrorCode);
        Assert.IsType<ApiException>(exception.InnerException);
        Assert.Equal(ApiFailureReason.HttpError, ((ApiException)exception.InnerException!).Reason);
    }

    [Fact]
    public async Task ObtenerPorPacienteAsync_WhenRemoteEnvelopeReportsFailure_ThrowsDownstreamException()
    {
        var payload = new DownstreamResponse<List<CitaDto>>
        {
            Succeeded = false,
            ErrorCode = 702,
            ErrorMessage = "Paciente desconocido"
        };
        using var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload))
        });
        var service = CreateService(client, "http://citas.local");

        var exception = await Assert.ThrowsAsync<DownstreamServiceException>(() => service.ObtenerPorPacienteAsync(7));

        Assert.Equal(701, exception.ErrorCode);
        Assert.Equal("Paciente desconocido", exception.Message);
    }

    [Fact]
    public async Task ObtenerPorPacienteAsync_WhenBaseUrlIsMissing_ExplainsConfigurationRequirement()
    {
        using var client = CreateClient(_ => throw new InvalidOperationException("No request should be sent."));
        var service = CreateService(client, "");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObtenerPorPacienteAsync(7));

        Assert.Contains("Downstream:CitasBaseUrl", exception.Message);
    }

    private static HttpClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        => new(new FakeHttpMessageHandler(responseFactory));

    private static CitasService CreateService(HttpClient client, string baseUrl)
    {
        var rest = new RestBuilder(client, Options.Create(new RequestSettings()));
        var options = Options.Create(new DownstreamOptions { CitasBaseUrl = baseUrl });
        return new CitasService(rest, options);
    }
}
