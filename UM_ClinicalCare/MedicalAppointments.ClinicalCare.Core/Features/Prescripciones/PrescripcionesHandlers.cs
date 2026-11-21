using MediatR;
using MedicalAppointments.ClinicalCare.Core.Contracts;
using MedicalAppointments.ClinicalCare.Domain.Entities;

namespace MedicalAppointments.ClinicalCare.Core.Features.Prescripciones;

public sealed record ListarPrescripcionesQuery(long? AtencionId, long? PacienteId) : IRequest<List<Prescripcion>>;
public sealed record ObtenerPrescripcionQuery(long Id) : IRequest<Prescripcion?>;
public sealed record CrearPrescripcionCommand(CrearPrescripcionRequest Datos) : IRequest<PrescripcionResult>;
public sealed record PrescripcionResult(Prescripcion? Prescripcion, string? Error);

public sealed class ListarPrescripcionesHandler(IClinicalCareRepository repository) : IRequestHandler<ListarPrescripcionesQuery, List<Prescripcion>>
{ public Task<List<Prescripcion>> Handle(ListarPrescripcionesQuery request, CancellationToken ct) => repository.ListarPrescripcionesAsync(request.AtencionId, request.PacienteId, ct); }

public sealed class ObtenerPrescripcionHandler(IClinicalCareRepository repository) : IRequestHandler<ObtenerPrescripcionQuery, Prescripcion?>
{ public Task<Prescripcion?> Handle(ObtenerPrescripcionQuery request, CancellationToken ct) => repository.ObtenerPrescripcionAsync(request.Id, ct); }

public sealed class CrearPrescripcionHandler(IClinicalCareRepository repository) : IRequestHandler<CrearPrescripcionCommand, PrescripcionResult>
{
    public async Task<PrescripcionResult> Handle(CrearPrescripcionCommand request, CancellationToken ct)
    {
        var atencion = await repository.ObtenerAtencionAsync(request.Datos.AtencionId, ct);
        if (atencion is null) return new(null, "No existe la atención indicada.");
        if (atencion.Estado != "Abierta") return new(null, "Solo se puede prescribir en una atención abierta.");
        if (atencion.PacienteId != request.Datos.PacienteId) return new(null, "El paciente no corresponde a la atención.");
        if (request.Datos.Detalles.Count == 0) return new(null, "Agrega al menos un medicamento a la prescripción.");
        var detalles = new List<PrescripcionDetalle>();
        foreach (var detalle in request.Datos.Detalles)
        {
            var medicamento = await repository.ObtenerMedicamentoAsync(detalle.MedicamentoId, ct);
            if (medicamento is null || !medicamento.Activo) return new(null, $"El medicamento {detalle.MedicamentoId} no existe o está inactivo.");
            detalles.Add(new PrescripcionDetalle { MedicamentoId = detalle.MedicamentoId, Dosis = detalle.Dosis,
                Frecuencia = detalle.Frecuencia, Duracion = detalle.Duracion, ViaAdministracion = detalle.ViaAdministracion });
        }
        var prescription = new Prescripcion { AtencionId = atencion.Id, PacienteId = atencion.PacienteId,
            IndicacionesGenerales = request.Datos.IndicacionesGenerales?.Trim(), Detalles = detalles };
        await repository.AgregarPrescripcionAsync(prescription, ct);
        return new(prescription, null);
    }
}
