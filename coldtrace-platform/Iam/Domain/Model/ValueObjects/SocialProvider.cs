namespace ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Supported external identity providers.
/// </summary>
public enum SocialProvider
{
    Google,
    Apple
}

/// <summary>
///     Parsing helpers for social provider route codes.
/// </summary>
public static class SocialProviderExtensions
{
    public static string ToCode(this SocialProvider provider) =>
        provider switch
        {
            SocialProvider.Google => "google",
            SocialProvider.Apple => "apple",
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
        };

    public static SocialProvider FromCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("iam.authentication.error.provider.required");

        return code.Trim().ToLowerInvariant() switch
        {
            "google" => SocialProvider.Google,
            "apple" => SocialProvider.Apple,
            _ => throw new ArgumentException("iam.authentication.error.provider.unsupported")
        };
    }
}
