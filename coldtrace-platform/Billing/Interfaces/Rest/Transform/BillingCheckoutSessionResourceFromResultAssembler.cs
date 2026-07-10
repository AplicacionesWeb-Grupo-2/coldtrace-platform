using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts checkout session application results into REST resources.
/// </summary>
public static class BillingCheckoutSessionResourceFromResultAssembler
{
    public static BillingCheckoutSessionResource ToResourceFromResult(BillingCheckoutSession session) =>
        new(
            session.Provider,
            session.SessionId,
            session.CheckoutUrl,
            session.TargetPlanCode);
}
