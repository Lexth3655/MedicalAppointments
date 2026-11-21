using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Features.Medicamentos;

public sealed record ListarMedicamentosQuery(bool IncluirInactivos = false) : IRequest<List<Medicamento>>;
public sealed record ObtenerMedicamentoQuery(long Id) : IRequest<Medicamento?>;
public sealed record CrearMedicamentoCommand(CrearMedicamentoRequest Datos) : IRequest<Medicamento?>;
public sealed record ActualizarMedicamentoCommand(long Id, ActualizarMedicamentoRequest Datos) : IRequest<Medicamento?>;
public sealed record CambiarEstadoMedicamentoCommand(long Id, bool Activo) : IRequest<Medicamento?>;

public sealed class ListarMedicamentosHandler(IClinicalCareRepository repository) : IRequestHandler<ListarMedicamentosQuery, List<Medicamento>>
{ public Task<List<Medicamento>> Handle(ListarMedicamentosQuery request, CancellationToken ct) => repository.ListarMedicamentosAsync(request.IncluirInactivos, ct); }

public sealed class ObtenerMedicamentoHandler(IClinicalCareRepository repository) : IRequestHandler<ObtenerMedicamentoQuery, Medicamento?>
{ public Task<Medicamento?> Handle(ObtenerMedicamentoQuery request, CancellationToken ct) => repository.ObtenerMedicamentoAsync(request.Id, ct); }

public sealed class CrearMedicamentoHandler(IClinicalCareRepository repository) : IRequestHandler<CrearMedicamentoCommand, Medicamento?>
{
    public async Task<Medicamento?> Handle(CrearMedicamentoCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Datos.Nombre)) return null;
        var item = new Medicamento { Nombre = request.Datos.Nombre.Trim(), PrincipioActivo = request.Datos.PrincipioActivo,
            Presentacion = request.Datos.Presentacion, Concentracion = request.Datos.Concentracion };
        await repository.AgregarMedicamentoAsync(item, ct);
        return item;
    }
}

public sealed class ActualizarMedicamentoHandler(IClinicalCareRepository repository) : IRequestHandler<ActualizarMedicamentoCommand, Medicamento?>
{
    public async Task<Medicamento?> Handle(ActualizarMedicamentoCommand request, CancellationToken ct)
    {
        var item = await repository.ObtenerMedicamentoAsync(request.Id, ct);
        if (item is null || string.IsNullOrWhiteSpace(request.Datos.Nombre)) return null;
        item.Nombre = request.Datos.Nombre.Trim(); item.PrincipioActivo = request.Datos.PrincipioActivo;
        item.Presentacion = request.Datos.Presentacion; item.Concentracion = request.Datos.Concentracion;
        await repository.GuardarMedicamentoAsync(item, ct); return item;
    }
}

public sealed class CambiarEstadoMedicamentoHandler(IClinicalCareRepository repository) : IRequestHandler<CambiarEstadoMedicamentoCommand, Medicamento?>
{
    public async Task<Medicamento?> Handle(CambiarEstadoMedicamentoCommand request, CancellationToken ct)
    {
        var item = await repository.ObtenerMedicamentoAsync(request.Id, ct);
        if (item is null) return null;
        item.Activo = request.Activo; await repository.GuardarMedicamentoAsync(item, ct); return item;
    }
}
