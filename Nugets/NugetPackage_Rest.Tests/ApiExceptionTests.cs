using NugetPackage_Rest.Exceptions;

namespace NugetPackage_Rest.Tests;

public class ApiExceptionTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var inner = new InvalidOperationException("boom");
        var uri = new Uri("https://api.example.com/orders");
        var exception = new ApiException("HTTP failure", ApiFailureReason.HttpError, inner, 422,
            "{\"error\":\"invalid\"}", HttpMethod.Post, uri);

        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(422, exception.StatusCode);
        Assert.Equal("{\"error\":\"invalid\"}", exception.ResponseBody);
        Assert.Equal(HttpMethod.Post, exception.RequestMethod);
        Assert.Equal(uri, exception.RequestUri);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Constructor_AllowsOptionalParametersToBeOmitted()
    {
        var exception = new ApiException("Timed out", ApiFailureReason.Timeout);
        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
        Assert.Null(exception.StatusCode);
        Assert.Null(exception.ResponseBody);
        Assert.Null(exception.RequestMethod);
        Assert.Null(exception.RequestUri);
        Assert.Null(exception.InnerException);
    }
}
