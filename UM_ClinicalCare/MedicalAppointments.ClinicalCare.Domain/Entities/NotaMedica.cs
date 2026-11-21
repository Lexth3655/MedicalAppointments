namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class NotaMedica
{
    public long Id { get; set; }
    public long AtencionId { get; set; }
    public Atencion Atencion { get; set; } = null!;
    public string Nota { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public long? MedicoId { get; set; }
    public string? CreadoPor { get; set; }
}
