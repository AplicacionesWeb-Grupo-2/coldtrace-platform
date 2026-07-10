namespace ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;

/// <summary>
///     Request sent to the external checkout provider adapter.
/// </summary>
public record CheckoutSessionProviderRequest(
    int OrganizationId,
    string TargetPlanCode,
    string StripePriceId,
    string? ProviderCustomerId);
