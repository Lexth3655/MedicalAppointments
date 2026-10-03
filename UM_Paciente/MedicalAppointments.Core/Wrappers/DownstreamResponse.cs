namespace MedicalAppointments.Patients.Core.Wrappers;

/// <summary>Sobre esperado para respuestas de los microservicios internos.</summary>
public sealed class DownstreamResponse<T>
{
    public bool Succeeded { get; set; }
    public T? Result { get; set; }
    public int ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
