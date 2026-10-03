using System.Runtime.CompilerServices;

namespace NugetPackage_Rest.Interfaces.IFluents;

/// <summary>Etapa final: ejecuta la solicitud y lee o deserializa la respuesta.</summary>
public interface IFluentContent
{
    Task<string> GetContentAsStringAsync();
    Task<byte[]> GetContentAsByteArrayAsync();
    Task<T> DeserializeWithAsync<T>();
    TaskAwaiter<HttpResponseMessage> GetAwaiter();
}
