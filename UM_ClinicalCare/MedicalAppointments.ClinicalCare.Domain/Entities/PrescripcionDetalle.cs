namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class PrescripcionDetalle
{
    public long Id { get; set; }
    public long PrescripcionId { get; set; }
    public Prescripcion Prescripcion { get; set; } = null!;
    public long MedicamentoId { get; set; }
    public Medicamento Medicamento { get; set; } = null!;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public string Duracion { get; set; } = string.Empty;
    public string ViaAdministracion { get; set; } = string.Empty;
}
