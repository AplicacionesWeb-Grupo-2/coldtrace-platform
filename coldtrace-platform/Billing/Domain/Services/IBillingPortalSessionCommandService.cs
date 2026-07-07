using ColdTrace.Platform.Billing.Application.Errors;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Domain.Services;

/// <summary>
///     Application service contract for customer portal session commands.
/// </summary>
public interface IBillingPortalSessionCommandService
{
    Task<Result<BillingPortalSession, BillingPortalSessionError>> Handle(
        CreateBillingPortalSessionCommand command,
        CancellationToken cancellationToken = default);
}
