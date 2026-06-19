namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

/// <summary>
///     Request body for creating a technical service request.
/// </summary>
public record CreateTechnicalServiceRequestResource(
    int AssetId,
    int? IncidentId,
    string IssueDescription,
    string Priority,
    string? RequestedBy);
