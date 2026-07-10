using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a role.
/// </summary>
[SwaggerSchema(Description = "A role resource with permissions")]
public record RoleResource(
    [SwaggerParameter(Description = "Role identifier")]
    int Id,
    [SwaggerParameter(Description = "Stable role name")]
    string Name,
    [SwaggerParameter(Description = "Role display label")]
    string Label,
    [SwaggerParameter(Description = "Permissions assigned to the role")]
    IEnumerable<PermissionResource> Permissions);
