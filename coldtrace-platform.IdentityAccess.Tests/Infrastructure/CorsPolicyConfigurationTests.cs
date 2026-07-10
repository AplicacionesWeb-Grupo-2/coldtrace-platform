using System.Net;
using ColdTrace.Platform.Shared.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class CorsPolicyConfigurationTests
{
    [Fact]
    public void ProductionWithoutConfiguredOrigins_FailsClosed()
    {
        var builder = CreateBuilder(Environments.Production, string.Empty);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.Services.AddColdTraceCors(builder.Configuration, builder.Environment));

        Assert.Contains(CorsPolicyConfiguration.AllowedOriginsConfigurationKey, exception.Message);
    }

    [Fact]
    public async Task ConfiguredOrigin_PreflightReturnsAllowOriginHeader()
    {
        const string allowedOrigin = "https://app.coldtrace.test";
        await using var app = await CreateApplicationAsync(Environments.Production, allowedOrigin);

        using var response = await SendPreflightAsync(app.GetTestClient(), allowedOrigin);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(allowedOrigin, response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    [Fact]
    public async Task UnlistedOrigin_PreflightDoesNotReturnAllowOriginHeader()
    {
        await using var app = await CreateApplicationAsync(
            Environments.Production,
            "https://app.coldtrace.test");

        using var response = await SendPreflightAsync(app.GetTestClient(), "https://untrusted.test");

        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Theory]
    [InlineData("http://localhost:5173")]
    [InlineData("http://127.0.0.1:5173")]
    public async Task DevelopmentWithoutConfiguredOrigins_AllowsLocalViteOrigins(string origin)
    {
        await using var app = await CreateApplicationAsync(Environments.Development, string.Empty);

        using var response = await SendPreflightAsync(app.GetTestClient(), origin);

        Assert.Equal(origin, response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    private static async Task<WebApplication> CreateApplicationAsync(string environmentName, string origins)
    {
        var builder = CreateBuilder(environmentName, origins);
        builder.Services.AddColdTraceCors(builder.Configuration, builder.Environment);
        var app = builder.Build();
        app.UseRouting();
        app.UseCors(CorsPolicyConfiguration.PolicyName);
        app.MapGet("/resource", () => Results.Ok());
        await app.StartAsync();
        return app;
    }

    private static WebApplicationBuilder CreateBuilder(string environmentName, string origins)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(CorsPolicyConfigurationTests).Assembly.FullName,
            EnvironmentName = environmentName
        });
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [CorsPolicyConfiguration.AllowedOriginsConfigurationKey] = origins
        });
        return builder;
    }

    private static Task<HttpResponseMessage> SendPreflightAsync(HttpClient client, string origin)
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/resource");
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "GET");
        return client.SendAsync(request);
    }
}
