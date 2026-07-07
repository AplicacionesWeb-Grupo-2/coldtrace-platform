namespace ColdTrace.Platform.Billing.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one organization's current subscription snapshot.
/// </summary>
public record GetOrganizationSubscriptionByOrganizationIdQuery
{
    public GetOrganizationSubscriptionByOrganizationIdQuery(int organizationId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));

        OrganizationId = organizationId;
    }

    public int OrganizationId { get; init; }
}
