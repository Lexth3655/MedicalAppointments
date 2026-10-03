using MedicalAppointments.Patients.Core.Wrappers;

namespace MedicalAppointments.Patients.Core.Services;

/// <summary>Contrato de negocio para consultar las citas de un paciente.</summary>
public interface ICitasService
{
    Task<IReadOnlyList<CitaDto>> ObtenerPorPacienteAsync(long pacienteId);
}
