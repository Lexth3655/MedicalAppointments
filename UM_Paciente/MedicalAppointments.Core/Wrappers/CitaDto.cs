namespace MedicalAppointments.Patients.Core.Wrappers;

/// <summary>Datos de cita recibidos desde el microservicio de Citas.</summary>
public sealed class CitaDto
{
    public long CitaId { get; set; }
    public long PacienteId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Especialidad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
