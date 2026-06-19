namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.REST.Resources;

/// <summary>
///     Request body for updating a technical service request status.
///     ClosureSummary, Evidence, and ClosedBy are required when Status is "closed".
/// </summary>
public record UpdateTechnicalServiceRequestStatusResource(
    string Status,
    string? ClosureSummary,
    string? Evidence,
    string? ClosedBy);
