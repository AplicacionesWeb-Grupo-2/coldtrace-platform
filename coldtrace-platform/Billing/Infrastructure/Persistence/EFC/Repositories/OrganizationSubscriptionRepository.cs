using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Billing.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for organization subscription persistence.
/// </summary>
public class OrganizationSubscriptionRepository(AppDbContext context)
    : BaseRepository<OrganizationSubscription>(context), IOrganizationSubscriptionRepository
{
    public async Task<OrganizationSubscription?> FindByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default) =>
        await Context.Set<OrganizationSubscription>()
            .FirstOrDefaultAsync(subscription => subscription.OrganizationId == organizationId, cancellationToken);

    public async Task<OrganizationSubscription?> FindByProviderCustomerIdAsync(
        string providerCustomerId,
        CancellationToken cancellationToken = default)
    {
        var normalizedProviderCustomerId = providerCustomerId.Trim();
        return await Context.Set<OrganizationSubscription>()
            .FirstOrDefaultAsync(
                subscription => subscription.ProviderCustomerId == normalizedProviderCustomerId,
                cancellationToken);
    }

    public async Task<OrganizationSubscription?> FindByProviderSubscriptionIdAsync(
        string providerSubscriptionId,
        CancellationToken cancellationToken = default)
    {
        var normalizedProviderSubscriptionId = providerSubscriptionId.Trim();
        return await Context.Set<OrganizationSubscription>()
            .FirstOrDefaultAsync(
                subscription => subscription.ProviderSubscriptionId == normalizedProviderSubscriptionId,
                cancellationToken);
    }
}
