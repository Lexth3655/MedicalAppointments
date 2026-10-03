using MedicalAppointments.Patients.Core.Configs;
using MedicalAppointments.Patients.Core.Exceptions;
using MedicalAppointments.Patients.Core.Services;
using MedicalAppointments.Patients.Core.Wrappers;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;

namespace MedicalAppointments.Patients.Infrastructure.Services;

/// <summary>Implementación HTTP del contrato de consultas de citas.</summary>
internal sealed class CitasService(IRest rest, IOptions<DownstreamOptions> options) : ICitasService
{
    private readonly DownstreamOptions _options = options.Value;

    public async Task<IReadOnlyList<CitaDto>> ObtenerPorPacienteAsync(long pacienteId)
    {
        if (string.IsNullOrWhiteSpace(_options.CitasBaseUrl))
            throw new InvalidOperationException("Configura Downstream:CitasBaseUrl antes de consultar las citas.");

        try
        {
            var response = await rest.Get
                .WithoutAuth()
                .WithUri(_options.CitasBaseUrl, $"/api/Citas/paciente/{pacienteId}")
                .DeserializeWithAsync<DownstreamResponse<List<CitaDto>>>();

            if (response is null || !response.Succeeded || response.Result is null)
                throw new DownstreamServiceException(response?.ErrorMessage ?? "El servicio de Citas devolvió una respuesta vacía.");

            return response.Result;
        }
        catch (ApiException ex)
        {
            throw new DownstreamServiceException("No fue posible consultar el microservicio de Citas.", innerException: ex);
        }
    }
}
