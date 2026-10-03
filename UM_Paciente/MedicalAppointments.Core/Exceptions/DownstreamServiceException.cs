namespace MedicalAppointments.Patients.Core.Exceptions;

/// <summary>Error de aplicación al consultar un microservicio interno.</summary>
public sealed class DownstreamServiceException : Exception
{
    public int ErrorCode { get; }

    public DownstreamServiceException(string message, int errorCode = 701, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
