using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Verified provider profile used to prefill organization onboarding.
/// </summary>
public record SocialIdentityProfileResource(
    [property: SwaggerSchema(Description = "Provider ID token verified by the backend")]
    string IdToken,
    [property: SwaggerSchema(Description = "Verified provider email")]
    string Email,
    [property: SwaggerSchema(Description = "Suggested full name")]
    string FullName);
