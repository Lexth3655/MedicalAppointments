namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class Atencion
{
    public long Id { get; set; }
    public long CitaId { get; set; }
    public long PacienteId { get; set; }
    public long MedicoId { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public string Diagnostico { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierta";
    public SignosVitales? SignosVitales { get; set; }
    public ICollection<NotaMedica> NotasMedicas { get; set; } = new List<NotaMedica>();
    public ICollection<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
}
