namespace ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;

/// <summary>
///     Errors that can occur when creating a technical service request.
/// </summary>
public enum CreateTechnicalServiceRequestError
{
    OrganizationNotFound,
    AssetNotFound,
    UnexpectedError
}
