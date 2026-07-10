using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

/// <summary>
///     Request payload for organization onboarding through a social provider.
/// </summary>
public record SocialOrganizationSignUpResource(
    [property: SwaggerSchema(Description = "Provider ID token")]
    string? IdToken,
    [property: SwaggerSchema(Description = "Provider authorization code")]
    string? AuthorizationCode,
    [property: SwaggerSchema(Description = "Redirect URI used during authorization-code flow")]
    string? RedirectUri,
    [property: SwaggerSchema(Description = "Optional nonce expected in the ID token")]
    string? Nonce,
    [property: Required]
    [property: SwaggerSchema(Description = "Organization name")]
    string OrganizationName,
    [property: Required]
    [property: SwaggerSchema(Description = "First user full name")]
    string FullName);
