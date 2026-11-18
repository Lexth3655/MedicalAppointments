using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.HorarioMedico.Commands;

public sealed record CrearHorarioCommand(long MedicoId, long ConsultorioId, DateOnly FechaDisponible,
    TimeOnly HoraInicio, TimeOnly HoraFin, int DuracionCitaMin = 30) : IRequest<HorariosMedicos>;

public sealed class CrearHorarioCommandHandler(IRepository<HorariosMedicos> repository)
    : IRequestHandler<CrearHorarioCommand, HorariosMedicos>
{
    public async Task<HorariosMedicos> Handle(CrearHorarioCommand request, CancellationToken cancellationToken)
    {
        var entity = new HorariosMedicos
        {
            MedicoId = request.MedicoId, ConsultorioId = request.ConsultorioId,
            FechaDisponible = request.FechaDisponible, HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin, DuracionCitaMin = request.DuracionCitaMin, Activo = true
        };
        await repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}
