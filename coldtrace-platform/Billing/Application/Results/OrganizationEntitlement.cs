namespace ColdTrace.Platform.Billing.Application.Results;

/// <summary>
///     One computed organization entitlement.
/// </summary>
public record OrganizationEntitlement(
    string Key,
    string Category,
    bool Enabled,
    int? Limit,
    int? Used,
    int? Remaining,
    string? LockedReason);
