namespace ColdTrace.Platform.Shared.Infrastructure.Configuration;

/// <summary>
///     Configures the browser origin allowlist for the API.
/// </summary>
public static class CorsPolicyConfiguration
{
    public const string PolicyName = "ColdTraceCorsPolicy";
    public const string AllowedOriginsConfigurationKey = "CORS_ALLOWED_ORIGINS";

    private static readonly string[] DevelopmentOrigins =
    [
        "http://localhost:5173",
        "http://127.0.0.1:5173"
    ];

    /// <summary>
    ///     Registers exact allowed origins from CORS_ALLOWED_ORIGINS.
    /// </summary>
    public static IServiceCollection AddColdTraceCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var allowedOrigins = ResolveAllowedOrigins(configuration, environment);
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Location")
                .SetPreflightMaxAge(TimeSpan.FromHours(1)));
        });

        return services;
    }

    private static string[] ResolveAllowedOrigins(IConfiguration configuration, IHostEnvironment environment)
    {
        var configuredOrigins = configuration[AllowedOriginsConfigurationKey]
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (configuredOrigins is { Length: > 0 })
        {
            if (configuredOrigins.Any(origin => origin.Contains('*', StringComparison.Ordinal)))
                throw new InvalidOperationException(
                    $"{AllowedOriginsConfigurationKey} must contain exact origins and cannot contain wildcards.");

            return configuredOrigins;
        }

        if (environment.IsDevelopment()) return DevelopmentOrigins;

        throw new InvalidOperationException(
            $"{AllowedOriginsConfigurationKey} must be configured outside the Development environment.");
    }
}
