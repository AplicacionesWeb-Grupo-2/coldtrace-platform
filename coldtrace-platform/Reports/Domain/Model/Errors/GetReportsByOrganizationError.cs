namespace ColdTrace.Platform.Reports.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving organization reports.
/// </summary>
public enum GetReportsByOrganizationError
{
    OrganizationNotFound,
    UnexpectedError
}
