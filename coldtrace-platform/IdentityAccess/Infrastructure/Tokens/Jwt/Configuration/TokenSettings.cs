namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     JWT token settings.
/// </summary>
public sealed class TokenSettings
{
    public const string SectionName = "TokenSettings";

    public string? Secret { get; set; }

    public int ExpirationDays { get; set; } = 7;

    public void ExpandEnvironmentVariables()
    {
        if (string.IsNullOrWhiteSpace(Secret)) return;

        var expandedSecret = Environment.ExpandEnvironmentVariables(Secret).Trim();
        Secret = expandedSecret.Contains('%') ||
                 expandedSecret.StartsWith("$", StringComparison.Ordinal)
            ? null
            : expandedSecret;
    }
}
