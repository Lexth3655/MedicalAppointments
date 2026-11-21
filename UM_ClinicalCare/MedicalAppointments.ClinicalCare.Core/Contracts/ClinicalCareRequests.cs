using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Contracts;

public sealed record ActualizarAtencionRequest(string Diagnostico);
public sealed record CambiarEstadoAtencionRequest(string Estado);
public sealed record GuardarSignosVitalesRequest(string? PresionArterial, decimal? FrecuenciaCardiaca,
    decimal? Temperatura, decimal? SaturacionOxigeno, decimal? Peso, decimal? Talla);
public sealed record CrearNotaMedicaRequest(string Nota);
public sealed record ActualizarNotaMedicaRequest(string Nota);
public sealed record CrearMedicamentoRequest(string Nombre, string? PrincipioActivo, string? Presentacion, string? Concentracion);
public sealed record ActualizarMedicamentoRequest(string Nombre, string? PrincipioActivo, string? Presentacion, string? Concentracion);
public sealed record CambiarEstadoMedicamentoRequest(bool Activo);
public sealed record CrearPrescripcionRequest(long AtencionId, long PacienteId, string? IndicacionesGenerales,
    IReadOnlyList<CrearPrescripcionDetalleRequest> Detalles);
public sealed record CrearPrescripcionDetalleRequest(long MedicamentoId, string Dosis, string Frecuencia, string Duracion, string ViaAdministracion);
public sealed record ValidacionCitaResult(bool Existe, bool Consistente, string? Mensaje);

public interface IClinicalCareExternalValidator
{
    Task<ValidacionCitaResult> ValidarCitaAsync(long citaId, long pacienteId, long medicoId, CancellationToken ct);
}
