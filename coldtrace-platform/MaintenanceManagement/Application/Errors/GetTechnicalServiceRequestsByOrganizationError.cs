namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when querying technical service requests by organization.
/// </summary>
public enum GetTechnicalServiceRequestsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
