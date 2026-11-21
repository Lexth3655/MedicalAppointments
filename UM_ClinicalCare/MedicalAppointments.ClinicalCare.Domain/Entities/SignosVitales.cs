namespace MedicalAppointments.ClinicalCare.Domain.Entities;

public sealed class SignosVitales
{
    public long Id { get; set; }
    public long AtencionId { get; set; }
    public Atencion Atencion { get; set; } = null!;
    public string? PresionArterial { get; set; }
    public decimal? FrecuenciaCardiaca { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? SaturacionOxigeno { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Talla { get; set; }
    public DateTime FechaRegistroUtc { get; set; } = DateTime.UtcNow;
}
