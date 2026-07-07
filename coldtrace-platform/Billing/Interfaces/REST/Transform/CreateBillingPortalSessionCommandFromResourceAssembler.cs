using ColdTrace.Platform.Billing.Domain.Model.Commands;

namespace ColdTrace.Platform.Billing.Interfaces.REST.Transform;

/// <summary>
///     Converts portal session route parameters into command objects.
/// </summary>
public static class CreateBillingPortalSessionCommandFromResourceAssembler
{
    public static CreateBillingPortalSessionCommand ToCommandFromResource(int organizationId) =>
        new(organizationId);
}
