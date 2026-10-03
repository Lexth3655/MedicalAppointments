using NugetPackage_Rest.Interfaces.IRequests;

namespace NugetPackage_Rest.Interfaces.IServices;

/// <summary>Punto de entrada inyectable de la librería REST.</summary>
public interface IRest
{
    INotContentRequest Get { get; }
    IWithContentRequest Post { get; }
    IWithContentRequest Put { get; }
    IWithContentRequest Delete { get; }
    IWithContentRequest Patch { get; }
}
