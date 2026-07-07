using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response resource for a provider-hosted checkout session.
/// </summary>
[SwaggerSchema(Description = "Provider-hosted checkout session redirect")]
public record BillingCheckoutSessionResource(
    [SwaggerParameter(Description = "Billing provider")]
    string Provider,
    [SwaggerParameter(Description = "Provider checkout session identifier")]
    string SessionId,
    [SwaggerParameter(Description = "Temporary provider-hosted checkout URL")]
    string CheckoutUrl,
    [SwaggerParameter(Description = "Target paid subscription plan code")]
    string TargetPlanCode);
