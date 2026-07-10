using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Billing.Interfaces.Rest.Transform;

/// <summary>
///     Converts checkout session REST resources into commands.
/// </summary>
public static class CreateBillingCheckoutSessionCommandFromResourceAssembler
{
    public static CreateBillingCheckoutSessionCommand ToCommandFromResource(
        int organizationId,
        CreateBillingCheckoutSessionResource resource) =>
        new(organizationId, resource.TargetPlanCode);
}
