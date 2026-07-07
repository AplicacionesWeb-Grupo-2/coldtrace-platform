using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Domain.Services;

/// <summary>
///     Application service contract for billing provider webhooks.
/// </summary>
public interface IBillingWebhookCommandService
{
    Task<Result<BillingWebhookProcessingResult, BillingWebhookError>> Handle(
        ProcessStripeWebhookCommand command,
        CancellationToken cancellationToken = default);
}
