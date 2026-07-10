using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Domain.Repositories;

/// <summary>
///     Organization subscription repository contract.
/// </summary>
public interface IOrganizationSubscriptionRepository : IBaseRepository<OrganizationSubscription>
{
    /// <summary>
    ///     Finds the current subscription for an organization.
    /// </summary>
    Task<OrganizationSubscription?> FindByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a subscription by external provider customer identifier.
    /// </summary>
    Task<OrganizationSubscription?> FindByProviderCustomerIdAsync(
        string providerCustomerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a subscription by external provider subscription identifier.
    /// </summary>
    Task<OrganizationSubscription?> FindByProviderSubscriptionIdAsync(
        string providerSubscriptionId,
        CancellationToken cancellationToken = default);
}
