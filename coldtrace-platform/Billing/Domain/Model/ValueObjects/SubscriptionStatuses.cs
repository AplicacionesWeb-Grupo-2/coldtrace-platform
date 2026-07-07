namespace ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Stable subscription lifecycle status values.
/// </summary>
public static class SubscriptionStatuses
{
    public const string Free = "FREE";
    public const string Active = "ACTIVE";
    public const string PastDue = "PAST_DUE";
    public const string Canceled = "CANCELED";

    public static bool AllowsPlanEntitlements(string status) =>
        string.Equals(status, Free, StringComparison.Ordinal) ||
        string.Equals(status, Active, StringComparison.Ordinal);
}
