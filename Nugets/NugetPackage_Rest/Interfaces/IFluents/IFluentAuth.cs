using System.Diagnostics.CodeAnalysis;

namespace NugetPackage_Rest.Interfaces.IFluents;

/// <summary>Etapa de autenticación, headers opcionales y URI obligatoria.</summary>
public interface IFluentAuth<TNext>
{
    TNext WithUri([NotNull] string uri, string endpoint = "");
    IFluentAuth<TNext> WithHeaders([NotNull] Dictionary<string, string> keyValues);
}
