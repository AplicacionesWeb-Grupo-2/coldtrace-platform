namespace ColdTrace.Platform.IdentityAccess.Application.Results;

/// <summary>
///     Verified social profile data used to prefill organization onboarding.
/// </summary>
public record SocialIdentityProfileResult(string IdToken, string Email, string FullName);
