namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;

/// <summary>
///     Request sent to the external customer portal provider adapter.
/// </summary>
public record PortalSessionProviderRequest(
    int OrganizationId,
    string ProviderCustomerId);
