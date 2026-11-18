using MedicalAppointments.Agenda.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Agenda.Domain.Models
{
    public class Citas
    {
        public long Id { get; set; }
        public long PacienteId { get; set; }
        public long MedicoId { get; set; }
        public Medicos Medico { get; set; } = null!;
        public long ConsultorioId { get; set; }
        public Consultorios Consultorio { get; set; } = null!;
        public DateTime FechaHora { get; set; }
        public string MotivoConsulta { get; set; } = string.Empty;
        public string Estado { get; set; } = EstadoCita.Programada.ToString();
        public string? Observaciones { get; set; }
        public string UsuarioReserva { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaModificacion { get; set; }


    }
}
