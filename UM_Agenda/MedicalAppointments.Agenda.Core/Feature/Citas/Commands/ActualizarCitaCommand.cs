using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Cita.Commands;

public sealed record ActualizarCitaCommand(long Id, long PacienteId, long MedicoId, long ConsultorioId,
    DateTime FechaHora, string MotivoConsulta, string? Observaciones, string UsuarioReserva) : IRequest<bool>;

public sealed class ActualizarCitaCommandHandler(IRepository<Citas> repository)
    : IRequestHandler<ActualizarCitaCommand, bool>
{
    public async Task<bool> Handle(ActualizarCitaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.PacienteId = request.PacienteId;
        entity.MedicoId = request.MedicoId;
        entity.ConsultorioId = request.ConsultorioId;
        entity.FechaHora = request.FechaHora;
        entity.MotivoConsulta = request.MotivoConsulta;
        entity.Observaciones = request.Observaciones ?? string.Empty;
        entity.UsuarioReserva = request.UsuarioReserva;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
