using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Contracts;

public interface IClinicalCareRepository
{
    Task<List<Atencion>> ListarAtencionesAsync(long? pacienteId, CancellationToken ct);
    Task<Atencion?> ObtenerAtencionAsync(long id, CancellationToken ct);
    Task<Atencion?> ObtenerAtencionPorCitaAsync(long citaId, CancellationToken ct);
    Task AgregarAtencionAsync(Atencion atencion, CancellationToken ct);
    Task GuardarAtencionAsync(Atencion atencion, CancellationToken ct);
    Task<List<SignosVitales>> ListarSignosVitalesAsync(long atencionId, CancellationToken ct);
    Task<SignosVitales?> ObtenerSignosVitalesAsync(long id, CancellationToken ct);
    Task AgregarSignosVitalesAsync(SignosVitales signos, CancellationToken ct);
    Task GuardarSignosVitalesAsync(SignosVitales signos, CancellationToken ct);
    Task<List<NotaMedica>> ListarNotasAsync(long atencionId, CancellationToken ct);
    Task<NotaMedica?> ObtenerNotaAsync(long id, CancellationToken ct);
    Task AgregarNotaAsync(NotaMedica nota, CancellationToken ct);
    Task GuardarNotaAsync(NotaMedica nota, CancellationToken ct);
    Task<List<Medicamento>> ListarMedicamentosAsync(bool incluirInactivos, CancellationToken ct);
    Task<Medicamento?> ObtenerMedicamentoAsync(long id, CancellationToken ct);
    Task AgregarMedicamentoAsync(Medicamento medicamento, CancellationToken ct);
    Task GuardarMedicamentoAsync(Medicamento medicamento, CancellationToken ct);
    Task<List<Prescripcion>> ListarPrescripcionesAsync(long? atencionId, long? pacienteId, CancellationToken ct);
    Task<Prescripcion?> ObtenerPrescripcionAsync(long id, CancellationToken ct);
    Task AgregarPrescripcionAsync(Prescripcion prescripcion, CancellationToken ct);
}
