using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;

/// <summary>
///     Outbound service for provider-hosted customer portal sessions.
/// </summary>
public interface IPortalSessionProviderService
{
    Task<Result<PortalSessionProviderResult, PortalSessionProviderFailure>> CreateCustomerPortalSessionAsync(
        PortalSessionProviderRequest request,
        CancellationToken cancellationToken = default);
}
