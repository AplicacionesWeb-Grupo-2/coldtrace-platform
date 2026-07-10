using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing one computed organization entitlement.
/// </summary>
[SwaggerSchema(Description = "A computed organization entitlement")]
public record OrganizationEntitlementResource(
    [SwaggerParameter(Description = "Stable entitlement key")]
    string Key,
    [SwaggerParameter(Description = "Entitlement category")]
    string Category,
    [SwaggerParameter(Description = "Whether the entitlement can be used now")]
    bool Enabled,
    [SwaggerParameter(Description = "Configured limit when this is a limit entitlement")]
    int? Limit,
    [SwaggerParameter(Description = "Current usage when this is a usage limit entitlement")]
    int? Used,
    [SwaggerParameter(Description = "Remaining capacity when this is a usage limit entitlement")]
    int? Remaining,
    [SwaggerParameter(Description = "Reason the entitlement is locked")]
    string? LockedReason);
