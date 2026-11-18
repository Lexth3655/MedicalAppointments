using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Agenda.Domain.Enums
{
    public enum EstadoCita
    {
        Programada = 1,
        Confirmada = 2,
        EnProgreso = 3,
        Atendida = 4,
        Cancelada = 5,
        noAsistio = 6
    }
}
