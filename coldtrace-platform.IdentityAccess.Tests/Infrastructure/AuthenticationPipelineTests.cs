using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Configuration;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Hashing.BCrypt.Services;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Configuration;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class AuthenticationPipelineTests
{
    private const string JwtSecret = "ColdTraceTestJwtSecretKeyForHS256WithAtLeast32Bytes";
    private const string OtherJwtSecret = "AnotherColdTraceTestJwtSecretForHS256AtLeast32Bytes";

    [Fact]
    public async Task PublicEndpoint_WithoutToken_ReturnsOk()
    {
        await using var app = await CreateApplicationAsync();

        using var response = await app.GetTestClient().GetAsync("/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorizedProblemDetails()
    {
        await using var app = await CreateApplicationAsync();

        using var response = await app.GetTestClient().GetAsync("/protected");

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized, "Unauthorized");
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.Single().Scheme);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithTokenSignedByAnotherKey_ReturnsUnauthorizedProblemDetails()
    {
        await using var app = await CreateApplicationAsync();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            GenerateToken(OtherJwtSecret));

        using var response = await client.GetAsync("/protected");

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized, "Unauthorized");
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidHs256Token_ReturnsOk()
    {
        await using var app = await CreateApplicationAsync();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken(JwtSecret));

        using var response = await client.GetAsync("/protected");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RestrictedEndpoint_WithAuthenticatedUser_ReturnsForbiddenProblemDetails()
    {
        await using var app = await CreateApplicationAsync();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateToken(JwtSecret));

        using var response = await client.GetAsync("/restricted");

        await AssertProblemDetailsAsync(response, HttpStatusCode.Forbidden, "Forbidden");
    }

    [Fact]
    public async Task OrganizationEndpoint_WithTokenFromSameOrganization_ReturnsOk()
    {
        await using var app = await CreateApplicationAsync();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            GenerateToken(JwtSecret, organizationId: 7));

        using var response = await client.GetAsync("/organizations/7/assets");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OrganizationEndpoint_WithTokenFromAnotherOrganization_ReturnsForbiddenProblemDetails()
    {
        await using var app = await CreateApplicationAsync();
        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            GenerateToken(JwtSecret, organizationId: 7));

        using var response = await client.GetAsync("/organizations/8/assets");

        await AssertProblemDetailsAsync(response, HttpStatusCode.Forbidden, "Forbidden");
    }

    private static async Task<WebApplication> CreateApplicationAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(AuthenticationPipelineTests).Assembly.FullName,
            EnvironmentName = Environments.Development
        });
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{TokenSettings.SectionName}:Secret"] = JwtSecret,
            [$"{TokenSettings.SectionName}:ExpirationDays"] = "7"
        });
        builder.Services.AddProblemDetails();
        builder.Services.AddColdTraceJwtBearerAuthentication(builder.Configuration);

        var app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGet("/public", () => Results.Ok()).AllowAnonymous();
        app.MapGet("/protected", () => Results.Ok());
        app.MapGet("/organizations/{organizationId:int}/assets", () => Results.Ok());
        app.MapGet("/restricted", () => Results.Ok())
            .RequireAuthorization(policy => policy.RequireClaim("permission", "restricted-resource"));
        await app.StartAsync();
        return app;
    }

    private static string GenerateToken(string secret, int organizationId = 7)
    {
        var command = new CreateUserCommand(
            "Cold",
            "Operator",
            "operator@coldtrace.test",
            "ColdTrace123",
            organizationId,
            3);
        var user = new User(command, new HashingService().HashPassword(command.Password));
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, 42);
        var tokenService = new TokenService(Options.Create(new TokenSettings
        {
            Secret = secret,
            ExpirationDays = 7
        }));
        return tokenService.GenerateToken(user);
    }

    private static async Task AssertProblemDetailsAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedTitle)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal((int)expectedStatus, problemDetails.Status);
        Assert.Equal(expectedTitle, problemDetails.Title);
    }
}
