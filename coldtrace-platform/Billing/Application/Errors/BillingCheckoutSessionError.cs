namespace ColdTrace.Platform.Billing.Application.Errors;

/// <summary>
///     Errors returned when creating provider-hosted checkout sessions.
/// </summary>
public enum BillingCheckoutSessionError
{
    OrganizationNotFound,
    OrganizationSubscriptionNotFound,
    TargetPlanNotFound,
    FreePlanCheckoutNotAllowed,
    PlanProviderPriceNotConfigured,
    ProviderNotConfigured,
    ProviderUnavailable,
    UnexpectedError
}
