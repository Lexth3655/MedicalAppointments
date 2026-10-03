namespace NugetPackage_Rest.Tests.Fakes;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage>? _responseFactory;
    private readonly Exception? _exceptionToThrow;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory) => _responseFactory = responseFactory;
    public FakeHttpMessageHandler(Exception exceptionToThrow) => _exceptionToThrow = exceptionToThrow;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_exceptionToThrow is not null) throw _exceptionToThrow;
        return Task.FromResult(_responseFactory!(request));
    }
}
