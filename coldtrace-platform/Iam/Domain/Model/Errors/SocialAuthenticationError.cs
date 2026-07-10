namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Stable application error returned by social authentication operations.
/// </summary>
/// <param name="Code">Client-facing error code.</param>
/// <param name="Message">Default client-facing message.</param>
/// <param name="Details">Bounded-context message key or contextual detail.</param>
public sealed record SocialAuthenticationError(string Code, string Message, string Details)
{
    public static SocialAuthenticationError ProviderValidationFailed() =>
        new(
            "PROVIDER_VALIDATION_FAILED",
            "Provider validation failed",
            "identity-access.authentication.error.provider-validation-failed");

    public static SocialAuthenticationError ProviderConfigurationMissing() =>
        new(
            "SOCIAL_PROVIDER_CONFIGURATION_MISSING",
            "Social provider configuration is missing",
            "identity-access.authentication.error.provider-configuration-missing");

    public static SocialAuthenticationError RequiresOnboarding() =>
        new(
            "SOCIAL_IDENTITY_REQUIRES_ONBOARDING",
            "Social identity requires onboarding",
            "identity-access.authentication.error.social-identity-requires-onboarding");

    public static SocialAuthenticationError UserNotFound(int userId) =>
        new("USER_NOT_FOUND", "User was not found", userId.ToString(System.Globalization.CultureInfo.InvariantCulture));

    public static SocialAuthenticationError UserConflict() =>
        new("USER_CONFLICT", "User conflict", "identity-access.user.error.email.duplicate");

    public static SocialAuthenticationError OrganizationConflict() =>
        new(
            "ORGANIZATION_CONFLICT",
            "Organization conflict",
            "identity-access.organization.error.contactEmail.duplicate");

    public static SocialAuthenticationError SocialIdentityConflict() =>
        new(
            "SOCIAL_IDENTITY_CONFLICT",
            "SocialIdentity conflict",
            "identity-access.authentication.error.social-identity-conflict");

    public static SocialAuthenticationError Unexpected(string details) =>
        new("UNEXPECTED_ERROR", "Unexpected error", details);
}
