using NugetPackage_Rest.Interfaces.IFluents;

namespace NugetPackage_Rest.Interfaces.IRequests;

/// <summary>Inicio de una solicitud sin body, normalmente GET.</summary>
public interface INotContentRequest
{
    IFluentAuth<IFluentContent> WithoutAuth();
    IFluentAuth<IFluentContent> WithBearer(string token);
    IFluentAuth<IFluentContent> WithBasic(string user, string password);
}
