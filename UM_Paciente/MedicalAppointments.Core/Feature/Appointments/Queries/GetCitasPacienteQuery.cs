using MediatR;
using MedicalAppointments.Patients.Core.Services;
using MedicalAppointments.Patients.Core.Wrappers;

namespace MedicalAppointments.Patients.Core.Feature.Appointments.Queries;

/// <summary>Consulta para obtener las citas asociadas a un paciente.</summary>
public sealed record GetCitasPacienteQuery(long PacienteId) : IRequest<IReadOnlyList<CitaDto>>;

public sealed class GetCitasPacienteQueryHandler(ICitasService citasService)
    : IRequestHandler<GetCitasPacienteQuery, IReadOnlyList<CitaDto>>
{
    public Task<IReadOnlyList<CitaDto>> Handle(GetCitasPacienteQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return citasService.ObtenerPorPacienteAsync(request.PacienteId);
    }
}
