using System.Diagnostics.CodeAnalysis;

namespace NugetPackage_Rest.Interfaces.IFluents;

/// <summary>Etapa que define el contenido de solicitudes con body.</summary>
public interface IFluentFormat
{
    IFluentContent WithoutBody();
    IFluentContent WithBody([NotNull] object body);
    IFluentContent WithFormData([NotNull] MultipartFormDataContent content);
    IFluentContent WithFormUrlEncoded([NotNull] IDictionary<string, string> data);
}
