using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Features.NotasMedicas;

public sealed record ListarNotasQuery(long AtencionId) : IRequest<List<NotaMedica>>;
public sealed record CrearNotaCommand(long AtencionId, string Nota, long? MedicoId, string? CreadoPor) : IRequest<NotaMedica?>;
public sealed record ActualizarNotaCommand(long Id, string Nota) : IRequest<NotaMedica?>;

public sealed class ListarNotasHandler(IClinicalCareRepository repository) : IRequestHandler<ListarNotasQuery, List<NotaMedica>>
{ public Task<List<NotaMedica>> Handle(ListarNotasQuery request, CancellationToken ct) => repository.ListarNotasAsync(request.AtencionId, ct); }

public sealed class CrearNotaHandler(IClinicalCareRepository repository) : IRequestHandler<CrearNotaCommand, NotaMedica?>
{
    public async Task<NotaMedica?> Handle(CrearNotaCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nota) || await repository.ObtenerAtencionAsync(request.AtencionId, ct) is null) return null;
        var nota = new NotaMedica { AtencionId = request.AtencionId, Nota = request.Nota.Trim(), MedicoId = request.MedicoId, CreadoPor = request.CreadoPor };
        await repository.AgregarNotaAsync(nota, ct);
        return nota;
    }
}

public sealed class ActualizarNotaHandler(IClinicalCareRepository repository) : IRequestHandler<ActualizarNotaCommand, NotaMedica?>
{
    public async Task<NotaMedica?> Handle(ActualizarNotaCommand request, CancellationToken ct)
    {
        var nota = await repository.ObtenerNotaAsync(request.Id, ct);
        if (nota is null || string.IsNullOrWhiteSpace(request.Nota)) return null;
        nota.Nota = request.Nota.Trim();
        await repository.GuardarNotaAsync(nota, ct);
        return nota;
    }
}
