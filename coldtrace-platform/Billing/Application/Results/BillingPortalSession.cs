namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     Provider-hosted customer portal session returned to the frontend.
/// </summary>
public record BillingPortalSession(
    string Provider,
    string SessionId,
    string PortalUrl,
    int OrganizationId);
