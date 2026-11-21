namespace MedicalAppointments.Security.Domain.Entities;

public sealed class Notificacion
{
    public long Id { get; set; }
    public Guid DestinatarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public long? CitaId { get; set; }
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
    public bool Leida { get; set; }
}
