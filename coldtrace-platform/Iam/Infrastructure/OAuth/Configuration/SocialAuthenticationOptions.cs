namespace ColdTrace.Platform.Iam.Infrastructure.OAuth.Configuration;

/// <summary>
///     Environment-driven Google and Apple social authentication configuration.
/// </summary>
public sealed class SocialAuthenticationOptions
{
    public const string SectionName = "Authentication:Social";

    public GoogleSocialAuthenticationOptions Google { get; set; } = new();

    public AppleSocialAuthenticationOptions Apple { get; set; } = new();

    public void ExpandEnvironmentVariables()
    {
        Google ??= new GoogleSocialAuthenticationOptions();
        Apple ??= new AppleSocialAuthenticationOptions();

        Google.ClientId = ExpandOptional(Google.ClientId);
        Google.ClientSecret = ExpandOptional(Google.ClientSecret);
        Google.RedirectUri = ExpandOptional(Google.RedirectUri);
        Google.TokenUri = ExpandOrDefault(
            Google.TokenUri,
            GoogleSocialAuthenticationOptions.DefaultTokenUri);
        Google.JwksUri = ExpandOrDefault(
            Google.JwksUri,
            GoogleSocialAuthenticationOptions.DefaultJwksUri);
        Google.Issuer = ExpandOrDefault(
            Google.Issuer,
            GoogleSocialAuthenticationOptions.DefaultIssuer);

        Apple.ClientId = ExpandOptional(Apple.ClientId);
        Apple.RedirectUri = ExpandOptional(Apple.RedirectUri);
        Apple.TokenUri = ExpandOrDefault(
            Apple.TokenUri,
            AppleSocialAuthenticationOptions.DefaultTokenUri);
        Apple.JwksUri = ExpandOrDefault(
            Apple.JwksUri,
            AppleSocialAuthenticationOptions.DefaultJwksUri);
        Apple.Issuer = ExpandOrDefault(
            Apple.Issuer,
            AppleSocialAuthenticationOptions.DefaultIssuer);
        Apple.TeamId = ExpandOptional(Apple.TeamId);
        Apple.KeyId = ExpandOptional(Apple.KeyId);
        Apple.PrivateKey = ExpandOptional(Apple.PrivateKey)?.Replace("\\n", "\n", StringComparison.Ordinal);
    }

    private static string ExpandOrDefault(string? value, string defaultValue) =>
        ExpandOptional(value) ?? defaultValue;

    private static string? ExpandOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var expanded = Environment.ExpandEnvironmentVariables(value).Trim();
        return IsUnexpandedEnvironmentReference(expanded) ? null : expanded;
    }

    private static bool IsUnexpandedEnvironmentReference(string value)
    {
        if (value.StartsWith('$')) return true;

        return value.Length > 2 &&
               value.StartsWith('%') &&
               value.EndsWith('%') &&
               value.IndexOf('%', 1) == value.Length - 1;
    }
}

public sealed class GoogleSocialAuthenticationOptions
{
    public const string DefaultTokenUri = "https://oauth2.googleapis.com/token";
    public const string DefaultJwksUri = "https://www.googleapis.com/oauth2/v3/certs";
    public const string DefaultIssuer = "https://accounts.google.com";

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string? RedirectUri { get; set; }

    public string TokenUri { get; set; } = DefaultTokenUri;

    public string JwksUri { get; set; } = DefaultJwksUri;

    public string Issuer { get; set; } = DefaultIssuer;
}

public sealed class AppleSocialAuthenticationOptions
{
    public const string DefaultTokenUri = "https://appleid.apple.com/auth/token";
    public const string DefaultJwksUri = "https://appleid.apple.com/auth/keys";
    public const string DefaultIssuer = "https://appleid.apple.com";

    public string? ClientId { get; set; }

    public string? RedirectUri { get; set; }

    public string TokenUri { get; set; } = DefaultTokenUri;

    public string JwksUri { get; set; } = DefaultJwksUri;

    public string Issuer { get; set; } = DefaultIssuer;

    public string? TeamId { get; set; }

    public string? KeyId { get; set; }

    public string? PrivateKey { get; set; }
}
