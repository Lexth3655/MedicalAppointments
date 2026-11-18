using MediatR;
using MedicalAppointments.Agenda.Domain.Models;
using NugetClass.Abstractions;

namespace MedicalAppointments.Agenda.Core.Feature.Consultorio.Commands;

public sealed class CrearConsultorioCommand : IRequest<Consultorios>
{
    public string Nombre { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string? Edificio_o_Nivel { get; set; }
    public int? Capacidad { get; set; }
}

public sealed class CrearConsultorioCommandHandler(IRepository<Consultorios> repository)
    : IRequestHandler<CrearConsultorioCommand, Consultorios>
{
    public async Task<Consultorios> Handle(CrearConsultorioCommand request, CancellationToken cancellationToken)
    {
        var entity = new Consultorios
        {
            Nombre = request.Nombre, Ubicacion = request.Ubicacion,
            Edificio_o_Nivel = request.Edificio_o_Nivel, Capacidad = request.Capacidad, Activo = true
        };
        await repository.AddAsync(entity, cancellationToken);
        return entity;
    }
}
