namespace ColdTrace.Platform.Billing.Domain.Model.Errors;

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
