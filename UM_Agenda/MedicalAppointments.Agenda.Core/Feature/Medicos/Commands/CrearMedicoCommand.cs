using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Medico.Commands;

public sealed class CrearMedicoCommand : IRequest<Medicos>
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CodigoMedico { get; set; } = string.Empty;
    public long EspecialidadId { get; set; }
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? TipoDocumento { get; set; }
}

public sealed class CrearMedicoCommandHandler(IRepository<Medicos> repository)
    : IRequestHandler<CrearMedicoCommand, Medicos>
{
    public async Task<Medicos> Handle(CrearMedicoCommand request, CancellationToken cancellationToken)
    {
        var entity = new Medicos
        {
            Nombre = request.Nombre, Apellido = request.Apellido, codigoMedico = request.CodigoMedico,
            EspecialidadId = request.EspecialidadId, Telefono = request.Telefono,
            CorreoElectronico = request.CorreoElectronico, numeroDocumento = request.NumeroDocumento,
            TipoDocumento = request.TipoDocumento, Activo = true
        };
        await repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}
