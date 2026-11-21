using System.Net.Http.Json;
using MedicalAppointments.ClinicalCare.Core.Contracts;

namespace MedicalAppointments.ClinicalCare.Infrastructure.Services;

internal sealed class AgendaClinicalCareValidator(HttpClient http) : IClinicalCareExternalValidator
{
    public async Task<ValidacionCitaResult> ValidarCitaAsync(long citaId, long pacienteId, long medicoId, CancellationToken ct)
    {
        try
        {
            using var response = await http.GetAsync($"api/citas/{citaId}", ct);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return new(false, false, "No se encontró la cita en Agenda.");
            response.EnsureSuccessStatusCode();
            var cita = await response.Content.ReadFromJsonAsync<CitaResponse>(cancellationToken: ct);
            if (cita is null) return new(false, false, "Agenda devolvió una cita vacía.");
            return new(true, cita.PacienteId == pacienteId && cita.MedicoId == medicoId,
                cita.PacienteId == pacienteId && cita.MedicoId == medicoId ? null : "Paciente o médico no coincide con la cita.");
        }
        catch (HttpRequestException) { return new(false, false, "Agenda no está disponible para validar la cita."); }
    }
    private sealed record CitaResponse(long Id, long PacienteId, long MedicoId);
}
