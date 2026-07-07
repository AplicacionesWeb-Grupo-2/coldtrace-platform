namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;

/// <summary>
///     Customer portal session created by an external provider.
/// </summary>
public record PortalSessionProviderResult(
    string Provider,
    string SessionId,
    string PortalUrl);
