using System.Net;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;

namespace NugetPackage_Rest.Tests;

public class HttpRequestExecutorTests
{
    [Fact]
    public async Task SendAsync_WhenHandlerThrowsHttpRequestException_MapsNetworkFailure()
    {
        var error = new HttpRequestException("Connection refused");
        using var client = new HttpClient(new FakeHttpMessageHandler(error));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

        var exception = await Assert.ThrowsAsync<ApiException>(() => HttpRequestExecutor.SendAsync(client, request));
        Assert.Equal(ApiFailureReason.Network, exception.Reason);
        Assert.Same(error, exception.InnerException);
    }

    [Fact]
    public async Task SendAsync_WhenHandlerThrowsTaskCanceledException_MapsTimeout()
    {
        var error = new TaskCanceledException("Timed out");
        using var client = new HttpClient(new FakeHttpMessageHandler(error));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");

        var exception = await Assert.ThrowsAsync<ApiException>(() => HttpRequestExecutor.SendAsync(client, request));
        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
        Assert.Same(error, exception.InnerException);
    }

    [Fact]
    public async Task SendAsync_WhenHandlerReturnsResponse_ReturnsSameResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => response));
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");
        Assert.Same(response, await HttpRequestExecutor.SendAsync(client, request));
    }

    [Fact]
    public async Task EnsureSuccessAsync_WhenStatusIsError_ThrowsWithStatusAndBody()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("{\"error\":\"missing\"}") };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders/42");
        var exception = await Assert.ThrowsAsync<ApiException>(() => HttpRequestExecutor.EnsureSuccessAsync(response, request));
        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("{\"error\":\"missing\"}", exception.ResponseBody);
    }

    [Fact]
    public async Task EnsureSuccessAsync_WhenStatusIsSuccess_DoesNotThrow()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");
        await HttpRequestExecutor.EnsureSuccessAsync(response, request);
    }
}
