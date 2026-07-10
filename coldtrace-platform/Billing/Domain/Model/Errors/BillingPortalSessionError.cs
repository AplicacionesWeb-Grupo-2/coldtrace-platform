namespace ColdTrace.Platform.Billing.Domain.Model.Errors;

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
