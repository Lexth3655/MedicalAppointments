using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Agenda.Persistence.Configs;

public sealed class MedicosConfiguration : IEntityTypeConfiguration<Medicos>
{
    public void Configure(EntityTypeBuilder<Medicos> builder)
    {
        builder.ToTable("Medicos", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("MedicoID");
        builder.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Apellido).HasColumnName("Apellido").HasMaxLength(100).IsRequired();
        builder.Property(x => x.codigoMedico).HasColumnName("CodigoMedico").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.HasIndex(x => x.codigoMedico).IsUnique().HasDatabaseName("UX_Medicos_Codigo");
        builder.Property(x => x.EspecialidadId).HasColumnName("EspecialidadID").IsRequired();
        builder.Property(x => x.Telefono).HasColumnName("Telefono").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.CorreoElectronico).HasColumnName("CorreoElectronico").HasMaxLength(150).IsUnicode(false);
        builder.Property(x => x.numeroDocumento).HasColumnName("NumeroDocumento").HasMaxLength(50).IsUnicode(false);
        builder.Property(x => x.TipoDocumento).HasColumnName("TipoDocumento").HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.HasOne(x => x.Especialidad).WithMany(x => x.Medicos).HasForeignKey(x => x.EspecialidadId).OnDelete(DeleteBehavior.NoAction);
    }
}
