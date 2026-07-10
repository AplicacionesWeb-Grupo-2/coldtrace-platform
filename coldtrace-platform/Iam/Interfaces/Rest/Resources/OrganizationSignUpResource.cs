using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Response resource for an organization sign-up.
/// </summary>
/// <param name="Organization">Created organization.</param>
/// <param name="User">First user created for the organization.</param>
[SwaggerSchema(Description = "An organization sign-up response resource")]
public record OrganizationSignUpResource(
    [SwaggerParameter(Description = "Created organization")]
    OrganizationResource Organization,
    [SwaggerParameter(Description = "Created first user")]
    UserResource User);
