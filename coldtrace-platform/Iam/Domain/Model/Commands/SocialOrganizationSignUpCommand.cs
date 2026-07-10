using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Iam.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization through a verified social identity.
/// </summary>
public record SocialOrganizationSignUpCommand
{
    public SocialOrganizationSignUpCommand(
        SocialProvider provider,
        string? idToken,
        string? authorizationCode,
        string? redirectUri,
        string? nonce,
        string organizationName,
        string fullName)
    {
        Provider = provider;
        IdToken = NormalizeOptional(idToken);
        AuthorizationCode = NormalizeOptional(authorizationCode);
        if (IdToken is null && AuthorizationCode is null)
            throw new ArgumentException("identity-access.authentication.error.social-token.required");
        RedirectUri = NormalizeOptional(redirectUri);
        Nonce = NormalizeOptional(nonce);
        OrganizationName = RequireNonBlank(
            organizationName,
            "identity-access.organization.error.commercialName.required");
        FullName = RequireNonBlank(fullName, "identity-access.user.error.firstName.required");
    }

    public SocialProvider Provider { get; init; }

    public string? IdToken { get; init; }

    public string? AuthorizationCode { get; init; }

    public string? RedirectUri { get; init; }

    public string? Nonce { get; init; }

    public string OrganizationName { get; init; }

    public string FullName { get; init; }

    public SocialSignInCommand ToSocialSignInCommand() =>
        new(Provider, IdToken, AuthorizationCode, RedirectUri, Nonce);

    private static string RequireNonBlank(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(message);
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
