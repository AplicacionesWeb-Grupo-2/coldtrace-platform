using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource for a provider-hosted customer portal session.
/// </summary>
[SwaggerSchema(Description = "Provider-hosted customer portal session redirect")]
public record BillingPortalSessionResource(
    [SwaggerParameter(Description = "Billing provider")]
    string Provider,
    [SwaggerParameter(Description = "Provider customer portal session identifier")]
    string SessionId,
    [SwaggerParameter(Description = "Temporary provider-hosted customer portal URL")]
    string PortalUrl,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId);
