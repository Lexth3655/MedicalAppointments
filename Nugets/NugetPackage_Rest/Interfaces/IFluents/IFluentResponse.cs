using System.Runtime.CompilerServices;

namespace NugetPackage_Rest.Interfaces.IFluents;

/// <summary>Contrato reservado que se conserva por compatibilidad con el paquete.</summary>
public interface IFluentResponse
{
    Task<string> GetContentAsStringAsync();
    Task<byte[]> GetContentAsByteArrayAsync();
    Task<T> DeserializeWithAsync<T>();
    TaskAwaiter<HttpResponseMessage> GetAwaiter();
}
