using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Provider token or authorization-code exchange payload.
/// </summary>
public record SocialTokenExchangeResource(
    [property: SwaggerSchema(Description = "Provider ID token returned by Google or Apple")]
    string? IdToken,
    [property: SwaggerSchema(Description = "Provider authorization code exchanged server-side")]
    string? AuthorizationCode,
    [property: SwaggerSchema(Description = "Redirect URI used in the OAuth request")]
    string? RedirectUri,
    [property: SwaggerSchema(Description = "Optional nonce expected in the provider ID token")]
    string? Nonce);
