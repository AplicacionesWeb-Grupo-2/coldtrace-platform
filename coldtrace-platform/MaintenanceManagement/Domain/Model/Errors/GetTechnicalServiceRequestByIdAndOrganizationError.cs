namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur when querying a technical service request by id.
/// </summary>
public enum GetTechnicalServiceRequestByIdAndOrganizationError
{
    OrganizationNotFound,
    TechnicalServiceRequestNotFound,
    UnexpectedError
}
