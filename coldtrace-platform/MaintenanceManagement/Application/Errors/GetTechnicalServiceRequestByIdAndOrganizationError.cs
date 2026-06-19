namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when querying a technical service request by id.
/// </summary>
public enum GetTechnicalServiceRequestByIdAndOrganizationError
{
    OrganizationNotFound,
    TechnicalServiceRequestNotFound,
    UnexpectedError
}
