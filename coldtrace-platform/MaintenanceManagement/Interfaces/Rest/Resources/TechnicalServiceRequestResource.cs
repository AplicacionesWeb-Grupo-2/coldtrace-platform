namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Resources;

/// <summary>
///     REST resource representing a technical service request.
/// </summary>
public record TechnicalServiceRequestResource(
    int Id,
    int OrganizationId,
    string Code,
    int AssetId,
    int AssetLocationId,
    string? AssetName,
    int? IncidentId,
    string IssueDescription,
    string Priority,
    string Status,
    string? RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ClosedAt,
    string? ClosureSummary,
    string? Evidence,
    string? ClosedBy,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);
