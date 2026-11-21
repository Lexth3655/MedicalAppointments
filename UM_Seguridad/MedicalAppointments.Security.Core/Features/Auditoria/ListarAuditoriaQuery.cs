using MediatR;
using MedicalAppointments.Security.Core.Contracts;

namespace MedicalAppointments.Security.Core.Features.Auditoria;

public sealed record ListarAuditoriaQuery(int Pagina, int Tamano, DateTime? DesdeUtc, DateTime? HastaUtc)
    : IRequest<ListarAuditoriaResult>;
public sealed record ListarAuditoriaResult(int Pagina, int Tamano, int Total, IReadOnlyList<AuditoriaResponse> Items);

public sealed class ListarAuditoriaQueryHandler(IAuditoriaRepository auditoria)
    : IRequestHandler<ListarAuditoriaQuery, ListarAuditoriaResult>
{
    public async Task<ListarAuditoriaResult> Handle(ListarAuditoriaQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await auditoria.ListarAsync(request.Pagina, request.Tamano, request.DesdeUtc, request.HastaUtc, cancellationToken);
        var result = items.Select(x => new AuditoriaResponse(x.Id, DateTime.SpecifyKind(x.FechaUtc, DateTimeKind.Utc), x.Metodo, x.Ruta, x.Recurso, x.Accion, x.Detalle, x.CodigoEstado,
            x.UsuarioId, x.Username, x.DireccionIp, x.AgenteUsuario, x.TraceId, x.DuracionMs)).ToList();
        return new(request.Pagina, request.Tamano, total, result);
    }
}
