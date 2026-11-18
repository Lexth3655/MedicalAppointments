using MedicalAppointments.Agenda.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace MedicalAppointments.Agenda.Persistence.Data
{
    public class AgendaContext(DbContextOptions<AgendaContext> options) : DbContext(options)
    {
        public DbSet<Citas> Citas => Set<Citas>();
        public DbSet<Consultorios> Consultorios => Set<Consultorios>();
        public DbSet<HorariosMedicos> HorariosMedicos => Set<HorariosMedicos>();
        public DbSet<Medicos> Medicos => Set<Medicos>();
        public DbSet<Especialidades> Especialidades => Set<Especialidades>();


        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendaContext).Assembly);
    }
}
