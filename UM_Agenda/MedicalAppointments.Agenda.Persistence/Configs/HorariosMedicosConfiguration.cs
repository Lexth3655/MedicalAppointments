using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Agenda.Persistence.Configs;

public sealed class HorariosMedicosConfiguration : IEntityTypeConfiguration<HorariosMedicos>
{
    public void Configure(EntityTypeBuilder<HorariosMedicos> builder)
    {
        builder.ToTable("HorariosMedicos", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("HorarioMedicoID");
        builder.Property(x => x.MedicoId).HasColumnName("MedicoID").IsRequired();
        builder.Property(x => x.ConsultorioId).HasColumnName("ConsultorioID").IsRequired();
        builder.Property(x => x.FechaDisponible).HasColumnName("FechaDisponible").HasColumnType("date").IsRequired();
        builder.Property(x => x.HoraInicio).HasColumnName("HoraInicio").HasColumnType("time").IsRequired();
        builder.Property(x => x.HoraFin).HasColumnName("HoraFin").HasColumnType("time").IsRequired();
        builder.Property(x => x.DuracionCitaMin).HasColumnName("DuracionCitaMin").HasDefaultValue(30);
        builder.Property(x => x.Activo).HasColumnName("Activo").HasDefaultValue(true);
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.FechaModificacion).HasColumnName("FechaModificacion").HasColumnType("datetime2").HasDefaultValueSql("SYSDATETIME()");
        builder.HasOne(x => x.Medico).WithMany(x => x.Horarios).HasForeignKey(x => x.MedicoId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Consultorio).WithMany(x => x.Horarios).HasForeignKey(x => x.ConsultorioId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => new { x.MedicoId, x.FechaDisponible }).HasDatabaseName("IX_HorariosMedicos_Medico_Fecha");
    }
}
