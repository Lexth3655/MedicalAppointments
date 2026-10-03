namespace MedicalAppointments.Patients.Core.Configs;

/// <summary>URLs de los microservicios que consume Pacientes.</summary>
public sealed class DownstreamOptions
{
    public string CitasBaseUrl { get; set; } = string.Empty;
}
