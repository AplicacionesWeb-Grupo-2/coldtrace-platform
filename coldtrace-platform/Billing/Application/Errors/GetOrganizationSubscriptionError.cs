namespace ColdTrace.Platform.Billing.Application.Errors;

/// <summary>
///     Errors returned when querying organization subscriptions.
/// </summary>
public enum GetOrganizationSubscriptionError
{
    OrganizationNotFound,
    OrganizationSubscriptionNotFound,
    SubscriptionPlanNotFound,
    UnexpectedError
}
