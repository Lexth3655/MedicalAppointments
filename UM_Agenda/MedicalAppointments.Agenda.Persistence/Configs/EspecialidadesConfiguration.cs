using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Agenda.Persistence.Configs;

public sealed class EspecialidadesConfiguration : IEntityTypeConfiguration<Especialidades>
{
    public void Configure(EntityTypeBuilder<Especialidades> builder)
    {
        builder.ToTable("Especialidades", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("EspecialidadID");
        builder.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(120).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UX_Especialidades_Nombre");
        builder.Property(x => x.Descripcion).HasColumnName("Descripcion").HasMaxLength(500);
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
    }
}
