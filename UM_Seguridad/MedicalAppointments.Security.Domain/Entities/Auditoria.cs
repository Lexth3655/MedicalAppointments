namespace MedicalAppointments.Security.Domain.Entities;

public sealed class Auditoria
{
    public long Id { get; set; }
    public DateTime FechaUtc { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string Recurso { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public int CodigoEstado { get; set; }
    public Guid? UsuarioId { get; set; }
    public string? Username { get; set; }
    public string? DireccionIp { get; set; }
    public string? AgenteUsuario { get; set; }
    public string? TraceId { get; set; }
    public long DuracionMs { get; set; }
}
