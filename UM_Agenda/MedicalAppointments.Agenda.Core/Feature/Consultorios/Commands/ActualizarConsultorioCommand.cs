using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Consultorio.Commands;

public sealed record ActualizarConsultorioCommand(long Id, string Nombre, string? Ubicacion,
    string? Edificio_o_Nivel, int? Capacidad) : IRequest<bool>;

public sealed class ActualizarConsultorioCommandHandler(IRepository<Consultorios> repository)
    : IRequestHandler<ActualizarConsultorioCommand, bool>
{
    public async Task<bool> Handle(ActualizarConsultorioCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Nombre = request.Nombre;
        entity.Ubicacion = request.Ubicacion;
        entity.Edificio_o_Nivel = request.Edificio_o_Nivel;
        entity.Capacidad = request.Capacidad;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
