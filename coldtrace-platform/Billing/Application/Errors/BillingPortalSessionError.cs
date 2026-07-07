namespace ColdTrace.Platform.Billing.Application.Errors;

/// <summary>
///     Errors returned when creating provider-hosted customer portal sessions.
/// </summary>
public enum BillingPortalSessionError
{
    OrganizationNotFound,
    OrganizationSubscriptionNotFound,
    ProviderCustomerNotFound,
    ProviderNotConfigured,
    ProviderUnavailable,
    UnexpectedError
}
