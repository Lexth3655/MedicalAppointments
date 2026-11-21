using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Features.SignosVitales;

using SignosVitalesEntity = MedicalAppointments.ClinicalCare.Domain.Entities.SignosVitales;

public sealed record ListarSignosVitalesQuery(long AtencionId) : IRequest<List<SignosVitalesEntity>>;
public sealed record GuardarSignosVitalesCommand(long AtencionId, GuardarSignosVitalesRequest Datos) : IRequest<SignosVitalesEntity?>;

public sealed class ListarSignosVitalesHandler(IClinicalCareRepository repository) : IRequestHandler<ListarSignosVitalesQuery, List<SignosVitalesEntity>>
{ public Task<List<SignosVitalesEntity>> Handle(ListarSignosVitalesQuery request, CancellationToken ct) => repository.ListarSignosVitalesAsync(request.AtencionId, ct); }

public sealed class GuardarSignosVitalesHandler(IClinicalCareRepository repository) : IRequestHandler<GuardarSignosVitalesCommand, SignosVitalesEntity?>
{
    public async Task<SignosVitalesEntity?> Handle(GuardarSignosVitalesCommand request, CancellationToken ct)
    {
        if (await repository.ObtenerAtencionAsync(request.AtencionId, ct) is null) return null;
        var signos = (await repository.ListarSignosVitalesAsync(request.AtencionId, ct)).FirstOrDefault()
            ?? new SignosVitalesEntity { AtencionId = request.AtencionId };
        signos.PresionArterial = request.Datos.PresionArterial;
        signos.FrecuenciaCardiaca = request.Datos.FrecuenciaCardiaca;
        signos.Temperatura = request.Datos.Temperatura;
        signos.SaturacionOxigeno = request.Datos.SaturacionOxigeno;
        signos.Peso = request.Datos.Peso;
        signos.Talla = request.Datos.Talla;
        if (signos.Id == 0) await repository.AgregarSignosVitalesAsync(signos, ct);
        else await repository.GuardarSignosVitalesAsync(signos, ct);
        return signos;
    }
}
