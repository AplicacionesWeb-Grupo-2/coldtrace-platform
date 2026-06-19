namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when updating a technical service request status.
/// </summary>
public enum UpdateTechnicalServiceRequestStatusError
{
    OrganizationNotFound,
    TechnicalServiceRequestNotFound,
    InvalidStatusTransition,
    UnexpectedError
}
