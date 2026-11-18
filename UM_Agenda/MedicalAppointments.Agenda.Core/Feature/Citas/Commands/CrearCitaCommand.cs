using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Commands;

public sealed class CrearCitaCommand : IRequest<Citas>
{
    public long PacienteId { get; set; }
    public long MedicoId { get; set; }
    public long ConsultorioId { get; set; }
    public DateTime FechaHora { get; set; }
    public string MotivoConsulta { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string UsuarioReserva { get; set; } = string.Empty;
}

public sealed class CrearCitaCommandHandler(IRepository<Citas> repository)
    : IRequestHandler<CrearCitaCommand, Citas>
{
    public async Task<Citas> Handle(CrearCitaCommand request, CancellationToken cancellationToken)
    {
        var entity = new Citas
        {
            PacienteId = request.PacienteId, MedicoId = request.MedicoId,
            ConsultorioId = request.ConsultorioId, FechaHora = request.FechaHora,
            MotivoConsulta = request.MotivoConsulta, Observaciones = request.Observaciones ?? string.Empty,
            UsuarioReserva = request.UsuarioReserva, Estado = "Programada", Activo = true
        };
        await repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}
