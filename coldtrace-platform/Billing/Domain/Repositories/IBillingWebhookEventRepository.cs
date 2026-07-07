using ColdTrace.Platform.Billing.Domain.Model.Entities;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Billing.Domain.Repositories;

/// <summary>
///     Domain repository contract for billing webhook event idempotency records.
/// </summary>
public interface IBillingWebhookEventRepository : IBaseRepository<BillingWebhookEvent>
{
    Task<bool> ExistsByProviderAndEventIdAsync(
        string provider,
        string eventId,
        CancellationToken cancellationToken = default);
}
