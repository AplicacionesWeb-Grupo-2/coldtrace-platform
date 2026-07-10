using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing current organization subscription usage.
/// </summary>
[SwaggerSchema(Description = "Current organization subscription usage counters")]
public record OrganizationSubscriptionUsageResource(
    [SwaggerParameter(Description = "Current location count")]
    int Locations,
    [SwaggerParameter(Description = "Current asset count")]
    int Assets,
    [SwaggerParameter(Description = "Current IoT device count")]
    int IotDevices,
    [SwaggerParameter(Description = "Current user count")]
    int Users);
