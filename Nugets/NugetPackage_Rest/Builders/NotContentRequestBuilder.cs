using Newtonsoft.Json;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IFluents;
using NugetPackage_Rest.Interfaces.IRequests;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace NugetPackage_Rest.Builders;

internal sealed class NotContentRequestBuilder : INotContentRequest, IFluentAuth<IFluentContent>, IFluentContent
{
    private readonly HttpClient _client;
    private readonly HttpRequestMessage _request;
    private readonly RequestSettings _settings;

    public NotContentRequestBuilder(HttpClient client, HttpMethod method, RequestSettings settings)
    {
        _client = client;
        _request = new HttpRequestMessage { Method = method };
        _settings = settings;
    }

    public IFluentAuth<IFluentContent> WithBasic(string user, string password)
    {
        var value = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
        _request.Headers.Authorization = new AuthenticationHeaderValue("Basic", value);
        return this;
    }

    public IFluentAuth<IFluentContent> WithBearer(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        _request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return this;
    }

    public IFluentAuth<IFluentContent> WithoutAuth() => this;

    public IFluentContent WithUri([NotNull] string uri, string endpoint = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        _request.RequestUri = new Uri($"{uri}{endpoint ?? string.Empty}", UriKind.Absolute);
        return this;
    }

    public IFluentAuth<IFluentContent> WithHeaders([NotNull] Dictionary<string, string> keyValues)
    {
        _request.AddHeaders(keyValues);
        return this;
    }

    public async Task<string> GetContentAsStringAsync()
    {
        var response = await SendAndValidateAsync().ConfigureAwait(false);
        return await HttpRequestExecutor.ReadContentAsStringAsync(response, _request).ConfigureAwait(false);
    }

    public async Task<byte[]> GetContentAsByteArrayAsync()
    {
        var response = await SendAndValidateAsync().ConfigureAwait(false);
        return await HttpRequestExecutor.ReadContentAsByteArrayAsync(response, _request).ConfigureAwait(false);
    }

    public async Task<T> DeserializeWithAsync<T>()
    {
        var response = await SendAndValidateAsync().ConfigureAwait(false);
        var content = await HttpRequestExecutor.ReadContentAsStringAsync(response, _request).ConfigureAwait(false);
        try { return JsonConvert.DeserializeObject<T>(content)!; }
        catch (JsonException ex)
        {
            throw HttpRequestExecutor.LogAndBuild($"Response from {_request.Method} {_request.RequestUri} could not be deserialized into {typeof(T).Name}: {ex.Message}", ApiFailureReason.Deserialization, _request, ex, responseBody: content);
        }
    }

    public TaskAwaiter<HttpResponseMessage> GetAwaiter() => _client.SendAsync(_request).GetAwaiter();

    private async Task<HttpResponseMessage> SendAndValidateAsync()
    {
        WriteRequestLog();
        var response = await HttpRequestExecutor.SendAsync(_client, _request).ConfigureAwait(false);
        await HttpRequestExecutor.EnsureSuccessAsync(response, _request).ConfigureAwait(false);
        return response;
    }

    private void WriteRequestLog()
    {
        if (_settings.EnableRequestLogs)
            Log.ForContext("Method", _request.Method).ForContext("Url", _request.RequestUri).Information("Sending request");
    }
}
