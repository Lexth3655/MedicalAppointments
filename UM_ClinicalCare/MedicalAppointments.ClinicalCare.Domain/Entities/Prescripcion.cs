namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class Prescripcion
{
    public long Id { get; set; }
    public long AtencionId { get; set; }
    public Atencion Atencion { get; set; } = null!;
    public long PacienteId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? IndicacionesGenerales { get; set; }
    public ICollection<PrescripcionDetalle> Detalles { get; set; } = new List<PrescripcionDetalle>();
}
