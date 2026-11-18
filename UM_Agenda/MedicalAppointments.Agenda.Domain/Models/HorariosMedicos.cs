using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Agenda.Domain.Models
{
    public class HorariosMedicos
    {
        public long Id { get; set; }
        public long MedicoId { get; set; }
        public Medicos Medico { get; set; } = null!;
        public long ConsultorioId { get; set; }
        public Consultorios Consultorio { get; set; } = null!;
        public DateOnly FechaDisponible { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public int DuracionCitaMin { get; set; } = 30;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; }

    }
}
