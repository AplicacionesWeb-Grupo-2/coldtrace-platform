namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     Current usage counters used to evaluate subscription limits.
/// </summary>
public record OrganizationSubscriptionUsage(
    int Locations,
    int Assets,
    int IotDevices,
    int Users);
