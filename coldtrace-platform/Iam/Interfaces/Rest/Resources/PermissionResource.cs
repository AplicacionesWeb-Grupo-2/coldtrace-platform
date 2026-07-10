using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a permission.
/// </summary>
[SwaggerSchema(Description = "A permission resource")]
public record PermissionResource(
    [SwaggerParameter(Description = "Permission identifier")]
    int Id,
    [SwaggerParameter(Description = "Protected resource name")]
    string Resource,
    [SwaggerParameter(Description = "Action allowed over the resource")]
    string Action,
    [SwaggerParameter(Description = "Permission description or translation key")]
    string Description);
