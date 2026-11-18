using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Agenda.Persistence.Configs;

public sealed class CitasConfiguration : IEntityTypeConfiguration<Citas>
{
    public void Configure(EntityTypeBuilder<Citas> builder)
    {
        builder.ToTable("Citas", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("CitaID");
        builder.Property(x => x.PacienteId).HasColumnName("PacienteID").IsRequired();
        // PacienteId identifica el paciente de su propio microservicio; aquí no hay FK local.
        builder.Property(x => x.MedicoId).HasColumnName("MedicoID").IsRequired();
        builder.Property(x => x.ConsultorioId).HasColumnName("ConsultorioID").IsRequired();
        builder.Property(x => x.FechaHora).HasColumnName("FechaHora").HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.MotivoConsulta).HasColumnName("MotivoConsulta").HasMaxLength(300).IsRequired();
        builder.Property(x => x.Estado).HasColumnName("Estado").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Observaciones).HasColumnName("Observaciones").HasMaxLength(1000);
        builder.Property(x => x.UsuarioReserva).HasColumnName("UsuarioReserva").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");

        builder.HasOne(x => x.Medico).WithMany(x => x.Citas).HasForeignKey(x => x.MedicoId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Consultorio).WithMany(x => x.Citas).HasForeignKey(x => x.ConsultorioId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => new { x.PacienteId, x.FechaHora }).HasDatabaseName("IX_Citas_Paciente_Fecha");
        builder.HasIndex(x => new { x.MedicoId, x.FechaHora }).HasDatabaseName("IX_Citas_Medico_Fecha");
    }
}
