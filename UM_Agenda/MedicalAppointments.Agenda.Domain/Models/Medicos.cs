using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Agenda.Domain.Models
{
    public class Medicos
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string codigoMedico { get; set; } = string.Empty;
        public long EspecialidadId { get; set; }
        public Especialidades Especialidad { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? numeroDocumento { get; set; }
        public string? TipoDocumento { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; }

        public ICollection<HorariosMedicos> Horarios { get; set; } = new List<HorariosMedicos>();
        public ICollection<Citas> Citas { get; set; } = new List<Citas>();

    }
}
