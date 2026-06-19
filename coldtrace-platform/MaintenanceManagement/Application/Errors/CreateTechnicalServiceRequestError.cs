namespace ColdTrace.Platform.MaintenanceManagement.Application.Errors;

/// <summary>
///     Errors that can occur when creating a technical service request.
/// </summary>
public enum CreateTechnicalServiceRequestError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
