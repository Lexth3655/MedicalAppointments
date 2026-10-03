using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;
using NugetPackage_Rest.Tests.Models;

namespace NugetPackage_Rest.Tests;

public class RestBuilderGetTests
{
    private static RestBuilder CreateRestBuilder(HttpClient client) => new(client, Options.Create(new RequestSettings()));

    [Fact]
    public async Task Get_WithSuccessResponse_ReturnsContentAsString()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello") }));
        var result = await CreateRestBuilder(client).Get.WithoutAuth().WithUri("https://api.example.com", "/orders").GetContentAsStringAsync();
        Assert.Equal("hello", result);
    }

    [Fact]
    public async Task Get_WithErrorStatusCode_ThrowsHttpErrorWithBody()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("missing") }));
        var exception = await Assert.ThrowsAsync<ApiException>(() => CreateRestBuilder(client).Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").GetContentAsStringAsync());
        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("missing", exception.ResponseBody);
    }

    [Fact]
    public async Task Get_WithValidJson_DeserializesIntoTargetType()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\":42,\"name\":\"widget\"}") }));
        var result = await CreateRestBuilder(client).Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").DeserializeWithAsync<TestOrder>();
        Assert.Equal(42, result.Id);
        Assert.Equal("widget", result.Name);
    }

    [Fact]
    public async Task Get_WithInvalidJson_ThrowsDeserializationErrorWithBody()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("not-json") }));
        var exception = await Assert.ThrowsAsync<ApiException>(() => CreateRestBuilder(client).Get.WithoutAuth().WithUri("https://api.example.com", "/orders/42").DeserializeWithAsync<TestOrder>());
        Assert.Equal(ApiFailureReason.Deserialization, exception.Reason);
        Assert.Equal("not-json", exception.ResponseBody);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task Get_WithBinaryResponse_ReturnsBytes()
    {
        var expected = Encoding.UTF8.GetBytes("binary-data");
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(expected) }));
        var result = await CreateRestBuilder(client).Get.WithoutAuth().WithUri("https://api.example.com", "/report.pdf").GetContentAsByteArrayAsync();
        Assert.Equal(expected, result);
    }
}
