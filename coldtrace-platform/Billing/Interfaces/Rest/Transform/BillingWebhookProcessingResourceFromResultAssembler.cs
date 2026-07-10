using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts billing webhook application results into REST resources.
/// </summary>
public static class BillingWebhookProcessingResourceFromResultAssembler
{
    public static BillingWebhookProcessingResource ToResourceFromResult(BillingWebhookProcessingResult result) =>
        new(
            result.Provider,
            result.EventId,
            result.EventType,
            result.ProcessingStatus,
            result.Duplicate,
            result.OrganizationId,
            result.PlanCode,
            result.SubscriptionStatus);
}
