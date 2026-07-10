namespace ColdTrace.Platform.Billing.Domain.Model.Commands;

/// <summary>
///     Command for ensuring an organization has its initial Base subscription.
/// </summary>
public record InitializeBaseOrganizationSubscriptionCommand
{
    public InitializeBaseOrganizationSubscriptionCommand(int organizationId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));

        OrganizationId = organizationId;
    }

    public int OrganizationId { get; init; }
}
