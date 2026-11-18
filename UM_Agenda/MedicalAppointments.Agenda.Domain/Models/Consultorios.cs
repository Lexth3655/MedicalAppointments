using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Agenda.Domain.Models
{
    public class Consultorios
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public string? Edificio_o_Nivel { get; set; }
        public int? Capacidad { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; }

        public ICollection<HorariosMedicos> Horarios { get; set; } = new List<HorariosMedicos>();
        public ICollection<Citas> Citas { get; set; } = new List<Citas>();

    }
}
