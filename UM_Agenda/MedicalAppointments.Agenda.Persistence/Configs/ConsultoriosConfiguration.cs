using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Agenda.Persistence.Configs;

public sealed class ConsultoriosConfiguration : IEntityTypeConfiguration<Consultorios>
{
    public void Configure(EntityTypeBuilder<Consultorios> builder)
    {
        builder.ToTable("Consultorios", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ConsultorioID");
        builder.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UX_Consultorios_Nombre");
        builder.Property(x => x.Ubicacion).HasColumnName("Ubicacion").HasMaxLength(200);
        builder.Property(x => x.Edificio_o_Nivel).HasColumnName("EdificioNivel").HasMaxLength(100);
        builder.Property(x => x.Capacidad).HasColumnName("Capacidad");
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
    }
}
