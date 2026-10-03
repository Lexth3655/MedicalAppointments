using NugetPackage_Rest.Interfaces.IFluents;

namespace NugetPackage_Rest.Interfaces.IRequests;

/// <summary>Inicio de una solicitud que permite definir un body.</summary>
public interface IWithContentRequest
{
    IFluentAuth<IFluentFormat> WithoutAuth();
    IFluentAuth<IFluentFormat> WithBearer(string token);
    IFluentAuth<IFluentFormat> WithBasic(string user, string password);
}
