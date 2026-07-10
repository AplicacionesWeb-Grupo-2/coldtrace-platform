using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Billing.Interfaces.Acl;

/// <summary>
///     Published anti-corruption facade for subscription and entitlement checks.
/// </summary>
public interface ISubscriptionBillingContextFacade
{
    const string EntitlementLocations = EntitlementKeys.Locations;
    const string EntitlementAssets = EntitlementKeys.Assets;
    const string EntitlementIotDevices = EntitlementKeys.IotDevices;
    const string EntitlementUsers = EntitlementKeys.Users;
    const string EntitlementReportHistory = EntitlementKeys.ReportHistory;
    const string EntitlementMaintenance = EntitlementKeys.Maintenance;
    const string EntitlementAiGuidance = EntitlementKeys.AiGuidance;
    const string EntitlementAiReportSummary = EntitlementKeys.AiReportSummary;

    Task InitializeBaseSubscriptionForOrganizationAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<OrganizationEntitlementsSnapshot?> FetchEntitlementsByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<EntitlementCheckSnapshot?> CheckEntitlementAsync(
        int organizationId,
        string entitlementKey,
        CancellationToken cancellationToken = default);

    Task<bool> CanUseEntitlementAsync(
        int organizationId,
        string entitlementKey,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     Subscription entitlement data published to other contexts.
/// </summary>
public record OrganizationEntitlementsSnapshot(
    int OrganizationId,
    string PlanCode,
    string Status,
    IReadOnlyCollection<EntitlementItemSnapshot> Entitlements);

/// <summary>
///     One entitlement item published to other contexts.
/// </summary>
public record EntitlementItemSnapshot(
    string Key,
    string Category,
    bool Enabled,
    int? Limit,
    int? Used,
    int? Remaining,
    string? LockedReason);

/// <summary>
///     One entitlement decision published to other contexts for enforcement.
/// </summary>
public record EntitlementCheckSnapshot(
    int OrganizationId,
    string? PlanCode,
    string? SubscriptionStatus,
    string Key,
    string? Category,
    bool Enabled,
    int? Limit,
    int? Used,
    int? Remaining,
    string? LockedReason,
    string? RequiredPlanCode);
