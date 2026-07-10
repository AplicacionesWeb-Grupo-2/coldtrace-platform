using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Resources;

/// <summary>
///     Request resource for creating a provider-hosted checkout session.
/// </summary>
[SwaggerSchema(Description = "Billing checkout session creation request")]
public record CreateBillingCheckoutSessionResource(
    [Required]
    [SwaggerParameter(Description = "Target paid subscription plan code")]
    string TargetPlanCode);
