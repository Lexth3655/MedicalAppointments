namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class Medicamento
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? PrincipioActivo { get; set; }
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<PrescripcionDetalle> PrescripcionesDetalle { get; set; } = new List<PrescripcionDetalle>();
}
