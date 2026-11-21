namespace MedicalAppointments.Security.Domain.Entities;

public sealed class PermisoRol
{
    public int Id { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
    public string Modulo { get; set; } = string.Empty;
    public bool Leer { get; set; }
    public bool Crear { get; set; }
    public bool Editar { get; set; }
    public bool Eliminar { get; set; }
}
