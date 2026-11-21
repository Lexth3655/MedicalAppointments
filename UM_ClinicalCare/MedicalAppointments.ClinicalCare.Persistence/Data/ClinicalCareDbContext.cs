using MedicalAppointments.ClinicalCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.ClinicalCare.Persistence.Data;

public sealed class ClinicalCareDbContext(DbContextOptions<ClinicalCareDbContext> options) : DbContext(options)
{
    public DbSet<Atencion> Atenciones => Set<Atencion>();
    public DbSet<SignosVitales> SignosVitales => Set<SignosVitales>();
    public DbSet<NotaMedica> NotasMedicas => Set<NotaMedica>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Prescripcion> Prescripciones => Set<Prescripcion>();
    public DbSet<PrescripcionDetalle> PrescripcionDetalle => Set<PrescripcionDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atencion>(entity =>
        {
            entity.ToTable("Atenciones"); entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CitaId).IsUnique();
            entity.Property(x => x.Diagnostico).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.Estado).HasMaxLength(20).IsRequired();
            entity.HasIndex(x => new { x.PacienteId, x.FechaHora });
        });
        modelBuilder.Entity<SignosVitales>(entity =>
        {
            entity.ToTable("SignosVitales"); entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AtencionId).IsUnique();
            entity.Property(x => x.PresionArterial).HasMaxLength(30);
            entity.Property(x => x.FrecuenciaCardiaca).HasPrecision(6, 2);
            entity.Property(x => x.Temperatura).HasPrecision(5, 2);
            entity.Property(x => x.SaturacionOxigeno).HasPrecision(5, 2);
            entity.Property(x => x.Peso).HasPrecision(6, 2);
            entity.Property(x => x.Talla).HasPrecision(6, 2);
            entity.HasOne(x => x.Atencion).WithOne(x => x.SignosVitales).HasForeignKey<SignosVitales>(x => x.AtencionId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<NotaMedica>(entity =>
        {
            entity.ToTable("NotasMedicas"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Nota).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.CreadoPor).HasMaxLength(80);
            entity.HasOne(x => x.Atencion).WithMany(x => x.NotasMedicas).HasForeignKey(x => x.AtencionId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.ToTable("Medicamentos"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PrincipioActivo).HasMaxLength(200);
            entity.Property(x => x.Presentacion).HasMaxLength(100);
            entity.Property(x => x.Concentracion).HasMaxLength(100);
            entity.HasIndex(x => x.Nombre);
        });
        modelBuilder.Entity<Prescripcion>(entity =>
        {
            entity.ToTable("Prescripciones"); entity.HasKey(x => x.Id);
            entity.Property(x => x.IndicacionesGenerales).HasMaxLength(2000);
            entity.HasOne(x => x.Atencion).WithMany(x => x.Prescripciones).HasForeignKey(x => x.AtencionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.PacienteId, x.Fecha });
        });
        modelBuilder.Entity<PrescripcionDetalle>(entity =>
        {
            entity.ToTable("PrescripcionDetalle"); entity.HasKey(x => x.Id);
            entity.Property(x => x.Dosis).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Frecuencia).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Duracion).HasMaxLength(150).IsRequired();
            entity.Property(x => x.ViaAdministracion).HasMaxLength(100).IsRequired();
            entity.HasOne(x => x.Prescripcion).WithMany(x => x.Detalles).HasForeignKey(x => x.PrescripcionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Medicamento).WithMany(x => x.PrescripcionesDetalle).HasForeignKey(x => x.MedicamentoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
