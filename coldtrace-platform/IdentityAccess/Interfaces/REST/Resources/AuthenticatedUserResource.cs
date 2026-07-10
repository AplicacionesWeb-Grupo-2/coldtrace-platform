using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

/// <summary>
///     Response resource returned after successful authentication.
/// </summary>
[SwaggerSchema(Description = "Authenticated user information with JWT token")]
public record AuthenticatedUserResource(
    [SwaggerParameter(Description = "User identifier")]
    int Id,
    [SwaggerParameter(Description = "Generated user code")]
    string Uuid,
    [SwaggerParameter(Description = "Organization-scoped user identifier")]
    int OrganizationUserId,
    [SwaggerParameter(Description = "User first name")]
    string FirstName,
    [SwaggerParameter(Description = "User last name")]
    string LastName,
    [SwaggerParameter(Description = "User email address")]
    string Email,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Role identifier")]
    int RoleId,
    [SwaggerParameter(Description = "JWT bearer token")]
    string Token);
