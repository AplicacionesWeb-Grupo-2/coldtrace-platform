using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Command for validating an external OIDC identity.
/// </summary>
public record SocialSignInCommand
{
    public SocialSignInCommand(
        SocialProvider provider,
        string? idToken,
        string? authorizationCode,
        string? redirectUri,
        string? nonce)
    {
        Provider = provider;
        IdToken = NormalizeOptional(idToken);
        AuthorizationCode = NormalizeOptional(authorizationCode);
        if (IdToken is null && AuthorizationCode is null)
            throw new ArgumentException("iam.authentication.error.social-token.required");
        RedirectUri = NormalizeOptional(redirectUri);
        Nonce = NormalizeOptional(nonce);
    }

    public SocialProvider Provider { get; init; }

    public string? IdToken { get; init; }

    public string? AuthorizationCode { get; init; }

    public string? RedirectUri { get; init; }

    public string? Nonce { get; init; }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
