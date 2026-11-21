using MedicalAppointments.Security.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Security.Persistence.Data;

public sealed class SecurityDbContext(DbContextOptions<SecurityDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<PermisoRol> PermisosRol => Set<PermisoRol>();
    public DbSet<Auditoria> Auditoria => Set<Auditoria>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => x.Nombre).IsUnique();
            entity.HasData(
                new Rol { Id = 1, Nombre = "Recepcion" },
                new Rol { Id = 2, Nombre = "Medico" },
                new Rol { Id = 3, Nombre = "Administrador" },
                new Rol { Id = 4, Nombre = "SupervisorTecnico" });
        });

        modelBuilder.Entity<PermisoRol>(entity =>
        {
            entity.ToTable("PermisosRol");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Modulo).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => new { x.RolId, x.Modulo }).IsUnique();
            entity.HasOne(x => x.Rol).WithMany(x => x.Permisos).HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Cascade);
            entity.HasData(CreateDefaultPermissions());
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(x => x.FechaCreacionUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasOne(x => x.Rol).WithMany(x => x.Usuarios).HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.MedicoId);
        });

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("Auditoria");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Metodo).HasMaxLength(12).IsRequired();
            entity.Property(x => x.Ruta).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Recurso).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Accion).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Detalle).HasMaxLength(1000);
            entity.Property(x => x.Username).HasMaxLength(80);
            entity.Property(x => x.DireccionIp).HasMaxLength(64);
            entity.Property(x => x.AgenteUsuario).HasMaxLength(512);
            entity.Property(x => x.TraceId).HasMaxLength(128);
            entity.HasIndex(x => x.FechaUtc);
            entity.HasIndex(x => x.UsuarioId);
            entity.HasIndex(x => new { x.CodigoEstado, x.FechaUtc });
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.ToTable("Notificaciones");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Titulo).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Mensaje).HasMaxLength(1200).IsRequired();
            entity.HasIndex(x => new { x.DestinatarioId, x.Leida, x.FechaUtc });
        });
    }

    private static IEnumerable<PermisoRol> CreateDefaultPermissions()
    {
        var modules = new[] { "dashboard", "patients", "agenda-appointments", "agenda-doctors", "agenda-specialties", "agenda-clinics", "agenda-schedules", "clinical-attentions", "clinical-vitals", "clinical-notes", "clinical-medicines", "clinical-prescriptions", "security-users", "security-roles", "security-audit", "patients-emergency-contacts" };
        var moduleIndexes = modules.Select((module, index) => (module, index)).ToDictionary(x => x.module, x => x.index);
        foreach (var module in modules)
        {
            var id = module == "patients-emergency-contacts" ? 1_000_001 : moduleIndexes[module] * 3 + 1;
            var receptionRead = module is "dashboard" or "patients" or "patients-emergency-contacts" or "agenda-appointments" or "agenda-doctors" or "agenda-specialties" or "agenda-clinics" or "agenda-schedules" or "clinical-attentions";
            var receptionCreate = module is "patients" or "patients-emergency-contacts" or "agenda-appointments";
            yield return new PermisoRol { Id = id++, RolId = 1, Modulo = module, Leer = receptionRead, Crear = receptionCreate };
            var doctorRead = module is "dashboard" or "patients" or "patients-emergency-contacts" or "agenda-appointments" or "agenda-doctors" or "agenda-clinics" or "agenda-schedules" or "clinical-attentions" or "clinical-vitals" or "clinical-notes" or "clinical-medicines" or "clinical-prescriptions";
            var doctorCreate = module.StartsWith("clinical-") || module is "patients-emergency-contacts" or "agenda-schedules";
            var doctorEdit = doctorCreate || module == "agenda-appointments";
            yield return new PermisoRol { Id = id++, RolId = 2, Modulo = module, Leer = doctorRead, Crear = doctorCreate, Editar = doctorEdit };
            yield return new PermisoRol { Id = id++, RolId = 3, Modulo = module, Leer = true, Crear = true, Editar = true, Eliminar = true };
            var supervisorRead = module is "dashboard" or "agenda-appointments" or "agenda-doctors" or "agenda-clinics" or "agenda-schedules" || module.StartsWith("clinical-");
            var supervisorManageMedicine = module == "clinical-medicines";
            var supervisorManageVitals = module == "clinical-vitals";
            yield return new PermisoRol { Id = 2_000_000 + id, RolId = 4, Modulo = module, Leer = supervisorRead || supervisorManageMedicine, Crear = module == "agenda-schedules" || supervisorManageMedicine || supervisorManageVitals, Editar = module == "agenda-schedules" || supervisorManageMedicine || supervisorManageVitals };
        }
    }
}
