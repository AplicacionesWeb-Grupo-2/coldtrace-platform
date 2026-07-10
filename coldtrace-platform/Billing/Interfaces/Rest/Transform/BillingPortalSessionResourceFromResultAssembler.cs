using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts customer portal session application results into REST resources.
/// </summary>
public static class BillingPortalSessionResourceFromResultAssembler
{
    public static BillingPortalSessionResource ToResourceFromResult(BillingPortalSession session) =>
        new(
            session.Provider,
            session.SessionId,
            session.PortalUrl,
            session.OrganizationId);
}
