using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Medico.Commands;

public sealed record ActualizarMedicoCommand(long Id, string Nombre, string Apellido, string CodigoMedico,
    long EspecialidadId, string? Telefono, string? CorreoElectronico, string? NumeroDocumento,
    string? TipoDocumento) : IRequest<bool>;

public sealed class ActualizarMedicoCommandHandler(IRepository<Medicos> repository)
    : IRequestHandler<ActualizarMedicoCommand, bool>
{
    public async Task<bool> Handle(ActualizarMedicoCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null) return false;
        entity.Nombre = request.Nombre;
        entity.Apellido = request.Apellido;
        entity.codigoMedico = request.CodigoMedico;
        entity.EspecialidadId = request.EspecialidadId;
        entity.Telefono = request.Telefono;
        entity.CorreoElectronico = request.CorreoElectronico;
        entity.numeroDocumento = request.NumeroDocumento;
        entity.TipoDocumento = request.TipoDocumento;
        entity.FechaModificacion = DateTime.UtcNow;
        return await repository.UpdateAsync(entity, cancellationToken);
    }
}
