using ColdTrace.Platform.Billing.Domain.Model.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Billing.Application.CommandServices;

/// <summary>
///     Application service contract for billing checkout sessions.
/// </summary>
public interface IBillingCheckoutSessionCommandService
{
    Task<Result<BillingCheckoutSession, BillingCheckoutSessionError>> Handle(
        CreateBillingCheckoutSessionCommand command,
        CancellationToken cancellationToken = default);
}
