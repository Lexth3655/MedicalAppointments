namespace NugetPackage_Rest.Exceptions;

/// <summary>Clasifica el motivo de una llamada HTTP fallida.</summary>
public enum ApiFailureReason
{
    Unknown,
    Network,
    Timeout,
    HttpError,
    Deserialization
}
