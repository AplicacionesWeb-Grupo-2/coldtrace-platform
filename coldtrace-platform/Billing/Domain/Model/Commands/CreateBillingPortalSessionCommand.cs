namespace ColdTrace.Platform.Billing.Domain.Model.Commands;

/// <summary>
///     Command to create a provider-hosted customer portal session.
/// </summary>
public record CreateBillingPortalSessionCommand
{
    public CreateBillingPortalSessionCommand(int organizationId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));

        OrganizationId = organizationId;
    }

    public int OrganizationId { get; init; }
}
