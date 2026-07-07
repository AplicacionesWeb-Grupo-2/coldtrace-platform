using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Domain.Services;

/// <summary>
///     Application service contract for billing checkout sessions.
/// </summary>
public interface IBillingCheckoutSessionCommandService
{
    Task<Result<BillingCheckoutSession, BillingCheckoutSessionError>> Handle(
        CreateBillingCheckoutSessionCommand command,
        CancellationToken cancellationToken = default);
}
