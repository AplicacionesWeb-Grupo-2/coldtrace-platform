using ColdTrace.Platform.Billing.Domain.Model.Entities;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for billing webhook event idempotency records.
/// </summary>
public class BillingWebhookEventRepository(AppDbContext context)
    : BaseRepository<BillingWebhookEvent>(context), IBillingWebhookEventRepository
{
    public async Task<bool> ExistsByProviderAndEventIdAsync(
        string provider,
        string eventId,
        CancellationToken cancellationToken = default)
    {
        var normalizedProvider = provider.Trim().ToUpperInvariant();
        var normalizedEventId = eventId.Trim();
        return await Context.Set<BillingWebhookEvent>().AnyAsync(
            webhookEvent => webhookEvent.Provider == normalizedProvider &&
                            webhookEvent.EventId == normalizedEventId,
            cancellationToken);
    }
}
