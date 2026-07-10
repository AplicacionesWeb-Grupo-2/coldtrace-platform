using System.Globalization;
using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Internal.OutboundServices.Social;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.Internal.CommandServices;

/// <summary>
///     Validates a provider identity and returns onboarding profile hints.
/// </summary>
public class SocialIdentityProfileCommandService(
    IExternalIdentityProviderService externalIdentityProviderService)
    : ISocialIdentityProfileCommandService
{
    /// <inheritdoc />
    public async Task<Result<SocialIdentityProfileResult, SocialAuthenticationError>> Handle(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default)
    {
        var providerResult = await externalIdentityProviderService.ValidateAsync(command, cancellationToken);
        if (providerResult is Result<ProviderIdentity, SocialAuthenticationError>.Failure failure)
            return new Result<SocialIdentityProfileResult, SocialAuthenticationError>.Failure(failure.Error);

        var identity = ((Result<ProviderIdentity, SocialAuthenticationError>.Success)providerResult).Value;
        var email = Normalize(identity.Email);
        var fullName = FirstNonBlank(
            identity.FullName,
            JoinName(identity.GivenName, identity.FamilyName),
            SuggestNameFromEmail(email));

        return new Result<SocialIdentityProfileResult, SocialAuthenticationError>.Success(
            new SocialIdentityProfileResult(identity.IdToken, email, fullName));
    }

    private static string FirstNonBlank(params string?[] values) =>
        values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? string.Empty;

    private static string JoinName(string? givenName, string? familyName) =>
        string.Join(' ', new[] { Normalize(givenName), Normalize(familyName) }
            .Where(value => value.Length > 0));

    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

    private static string SuggestNameFromEmail(string email)
    {
        var separatorIndex = email.IndexOf('@', StringComparison.Ordinal);
        if (separatorIndex <= 0) return string.Empty;

        var words = email[..separatorIndex]
            .Split(['.', '_', '-'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return string.Join(' ', words.Select(word =>
            string.Concat(
                char.ToUpper(word[0], CultureInfo.InvariantCulture),
                word[1..].ToLower(CultureInfo.InvariantCulture))));
    }
}
