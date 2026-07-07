using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Billing.Domain.Model.Aggregates;

/// <summary>
///     Current billing subscription owned by one organization.
/// </summary>
public class OrganizationSubscription : IAuditableEntity
{
    private const string BasePlanCode = "base";

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected OrganizationSubscription()
    {
        PlanCode = BasePlanCode;
        Status = SubscriptionStatuses.Free;
        Provider = BillingProviders.None;
    }

    /// <summary>
    ///     Creates the initial Base subscription for an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    public OrganizationSubscription(int organizationId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));

        OrganizationId = organizationId;
        PlanCode = BasePlanCode;
        Status = SubscriptionStatuses.Free;
        Provider = BillingProviders.None;
        CancelAtPeriodEnd = false;
    }

    /// <summary>
    ///     Gets the server-generated subscription identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the owning organization identifier.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the owning organization.
    /// </summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>
    ///     Gets the active plan code.
    /// </summary>
    public string PlanCode { get; private set; }

    /// <summary>
    ///     Gets the subscription lifecycle status.
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    ///     Gets the external billing provider.
    /// </summary>
    public string Provider { get; private set; }

    /// <summary>
    ///     Gets the external provider customer identifier.
    /// </summary>
    public string? ProviderCustomerId { get; private set; }

    /// <summary>
    ///     Gets the external provider subscription identifier.
    /// </summary>
    public string? ProviderSubscriptionId { get; private set; }

    /// <summary>
    ///     Gets the current billing period start.
    /// </summary>
    public DateTimeOffset? CurrentPeriodStart { get; private set; }

    /// <summary>
    ///     Gets the current billing period end.
    /// </summary>
    public DateTimeOffset? CurrentPeriodEnd { get; private set; }

    /// <summary>
    ///     Gets whether cancellation is scheduled at the end of the current billing period.
    /// </summary>
    public bool CancelAtPeriodEnd { get; private set; }

    /// <summary>
    ///     Gets non-sensitive provider synchronization metadata.
    /// </summary>
    public string? Metadata { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    ///     Indicates whether this subscription status can unlock plan entitlements.
    /// </summary>
    /// <returns>True when plan limits and feature flags are available.</returns>
    public bool AllowsPlanEntitlements() => SubscriptionStatuses.AllowsPlanEntitlements(Status);
}
