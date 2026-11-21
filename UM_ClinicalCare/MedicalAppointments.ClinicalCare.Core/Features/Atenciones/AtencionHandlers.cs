using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Features.Atenciones;

public sealed record ListarAtencionesQuery(long? PacienteId) : IRequest<List<Atencion>>;
public sealed record ObtenerAtencionQuery(long Id) : IRequest<Atencion?>;
public sealed record CrearAtencionCommand(long CitaId, long PacienteId, long MedicoId, string Diagnostico) : IRequest<AtencionResult>;
public sealed record ActualizarAtencionCommand(long Id, string Diagnostico) : IRequest<Atencion?>;
public sealed record CambiarEstadoAtencionCommand(long Id, string Estado) : IRequest<Atencion?>;
public sealed record AtencionResult(Atencion? Atencion, string? Error, bool Conflicto = false);

public sealed class ListarAtencionesHandler(IClinicalCareRepository repository) : IRequestHandler<ListarAtencionesQuery, List<Atencion>>
{ public Task<List<Atencion>> Handle(ListarAtencionesQuery request, CancellationToken ct) => repository.ListarAtencionesAsync(request.PacienteId, ct); }

public sealed class ObtenerAtencionHandler(IClinicalCareRepository repository) : IRequestHandler<ObtenerAtencionQuery, Atencion?>
{ public Task<Atencion?> Handle(ObtenerAtencionQuery request, CancellationToken ct) => repository.ObtenerAtencionAsync(request.Id, ct); }

public sealed class CrearAtencionHandler(IClinicalCareRepository repository, IClinicalCareExternalValidator validator)
    : IRequestHandler<CrearAtencionCommand, AtencionResult>
{
    public async Task<AtencionResult> Handle(CrearAtencionCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Diagnostico)) return new(null, "El diagnóstico es obligatorio.");
        if (await repository.ObtenerAtencionPorCitaAsync(request.CitaId, ct) is not null) return new(null, "La cita ya tiene una atención registrada.", true);
        var validacion = await validator.ValidarCitaAsync(request.CitaId, request.PacienteId, request.MedicoId, ct);
        if (!validacion.Existe) return new(null, validacion.Mensaje ?? "No se encontró la cita en Agenda.");
        if (!validacion.Consistente) return new(null, validacion.Mensaje ?? "Los identificadores no coinciden con la cita.");
        var atencion = new Atencion { CitaId = request.CitaId, PacienteId = request.PacienteId, MedicoId = request.MedicoId, Diagnostico = request.Diagnostico.Trim() };
        await repository.AgregarAtencionAsync(atencion, ct);
        return new(atencion, null);
    }
}

public sealed class ActualizarAtencionHandler(IClinicalCareRepository repository) : IRequestHandler<ActualizarAtencionCommand, Atencion?>
{
    public async Task<Atencion?> Handle(ActualizarAtencionCommand request, CancellationToken ct)
    {
        var atencion = await repository.ObtenerAtencionAsync(request.Id, ct);
        if (atencion is null || atencion.Estado == "Cerrada") return null;
        atencion.Diagnostico = request.Diagnostico.Trim();
        await repository.GuardarAtencionAsync(atencion, ct);
        return atencion;
    }
}

public sealed class CambiarEstadoAtencionHandler(IClinicalCareRepository repository) : IRequestHandler<CambiarEstadoAtencionCommand, Atencion?>
{
    public async Task<Atencion?> Handle(CambiarEstadoAtencionCommand request, CancellationToken ct)
    {
        var atencion = await repository.ObtenerAtencionAsync(request.Id, ct);
        if (atencion is null || request.Estado is not ("Abierta" or "Cerrada")) return null;
        atencion.Estado = request.Estado;
        await repository.GuardarAtencionAsync(atencion, ct);
        return atencion;
    }
}
