using System.Text;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Configuration;

/// <summary>
///     Configures JWT bearer authentication and the API's authenticated-by-default policy.
/// </summary>
public static class JwtBearerAuthenticationExtensions
{
    /// <summary>
    ///     Registers HS256 bearer token validation using the same settings as token issuance.
    /// </summary>
    public static IServiceCollection AddColdTraceJwtBearerAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<TokenSettings>()
            .Bind(configuration.GetSection(TokenSettings.SectionName))
            .PostConfigure(options => options.ExpandEnvironmentVariables())
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Secret) &&
                           Encoding.UTF8.GetByteCount(options.Secret) >= 32,
                "JWT secret must be configured with at least 32 bytes.")
            .Validate(options => options.ExpirationDays > 0, "JWT expiration days must be positive.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<TokenSettings>>((options, tokenSettings) =>
                ConfigureJwtBearerOptions(options, tokenSettings.Value));

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, TokenSettings tokenSettings)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret!));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Headers.WWWAuthenticate = JwtBearerDefaults.AuthenticationScheme;
                return WriteProblemDetailsAsync(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "A valid bearer token is required to access this resource.");
            },
            OnForbidden = context => WriteProblemDetailsAsync(
                context.HttpContext,
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "The authenticated user is not allowed to access this resource.")
        };
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        httpContext.Response.StatusCode = statusCode;
        var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        return problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = $"{httpContext.Request.PathBase}{httpContext.Request.Path}"
            }
        }).AsTask();
    }
}
