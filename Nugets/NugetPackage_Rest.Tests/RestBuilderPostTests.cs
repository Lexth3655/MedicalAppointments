using System.Net;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;
using NugetPackage_Rest.Tests.Models;

namespace NugetPackage_Rest.Tests;

public class RestBuilderPostTests
{
    private static RestBuilder CreateRestBuilder(HttpClient client) => new(client, Options.Create(new RequestSettings()));

    [Fact]
    public async Task Post_WithSuccessResponse_ReturnsContentAsString()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("created") }));
        var result = await CreateRestBuilder(client).Post.WithoutAuth().WithUri("https://api.example.com", "/orders").WithBody(new { Name = "widget" }).GetContentAsStringAsync();
        Assert.Equal("created", result);
    }

    [Fact]
    public async Task Post_WithUnprocessableEntity_ThrowsHttpErrorWithBody()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage((HttpStatusCode)422) { Content = new StringContent("{\"error\":\"required\"}") }));
        var exception = await Assert.ThrowsAsync<ApiException>(() => CreateRestBuilder(client).Post.WithoutAuth().WithUri("https://api.example.com", "/orders").WithBody(new { Name = "" }).GetContentAsStringAsync());
        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(422, exception.StatusCode);
        Assert.Equal("{\"error\":\"required\"}", exception.ResponseBody);
    }

    [Fact]
    public async Task Post_WithValidJsonResponse_DeserializesIntoTargetType()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"id\":7,\"name\":\"widget\"}") }));
        var result = await CreateRestBuilder(client).Post.WithoutAuth().WithUri("https://api.example.com", "/orders").WithBody(new { Name = "widget" }).DeserializeWithAsync<TestOrder>();
        Assert.Equal(7, result.Id);
        Assert.Equal("widget", result.Name);
    }

    [Fact]
    public async Task Post_WithoutBody_SendsSuccessfully()
    {
        using var client = new HttpClient(new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") }));
        var result = await CreateRestBuilder(client).Post.WithoutAuth().WithUri("https://api.example.com", "/sync").WithoutBody().GetContentAsStringAsync();
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Post_WithFormUrlEncoded_SendsExpectedContentTypeAndBody()
    {
        HttpRequestMessage? captured = null;
        using var client = new HttpClient(new FakeHttpMessageHandler(request =>
        {
            captured = request;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        }));
        await CreateRestBuilder(client).Post.WithoutAuth().WithUri("https://api.example.com", "/auth")
            .WithFormUrlEncoded(new Dictionary<string, string> { ["user"] = "student 01", ["password"] = "secret" })
            .GetContentAsStringAsync();

        Assert.NotNull(captured?.Content);
        Assert.Equal("application/x-www-form-urlencoded", captured!.Content!.Headers.ContentType!.MediaType);
        Assert.Equal("user=student+01&password=secret", await captured.Content.ReadAsStringAsync());
    }
}
