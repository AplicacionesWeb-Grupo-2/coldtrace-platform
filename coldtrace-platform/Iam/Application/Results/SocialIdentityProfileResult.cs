namespace ColdTrace.Platform.Iam.Application.Results;

/// <summary>
///     Verified social profile data used to prefill organization onboarding.
/// </summary>
public record SocialIdentityProfileResult(string IdToken, string Email, string FullName);
