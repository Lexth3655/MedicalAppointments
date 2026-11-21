namespace MedicalAppointments.Security.Domain.Entities;

public sealed class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<PermisoRol> Permisos { get; set; } = new List<PermisoRol>();
}
