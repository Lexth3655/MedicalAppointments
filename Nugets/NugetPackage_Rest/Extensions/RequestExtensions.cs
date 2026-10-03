using Newtonsoft.Json;
using System.Text;

namespace NugetPackage_Rest.Extensions;

/// <summary>Helpers para agregar headers y cuerpos HTTP.</summary>
public static class RequestExtensions
{
    public static void AddHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(headers);
        foreach (var header in headers)
        {
            if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                throw new ArgumentException($"'{header.Key}' no es un header válido para la solicitud.", nameof(headers));
        }
    }

    public static void AddContent(this HttpRequestMessage request, object body)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(body);
        request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
    }

    public static void AddFormDataContent(this HttpRequestMessage request, MultipartFormDataContent content)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(content);
        request.Content = content;
    }

    public static void AddFormUrlEncodedContent(this HttpRequestMessage request, IDictionary<string, string> data)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(data);
        request.Content = new FormUrlEncodedContent(data);
    }
}
