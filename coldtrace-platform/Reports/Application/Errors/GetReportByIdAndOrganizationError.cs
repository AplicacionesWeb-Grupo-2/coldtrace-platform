namespace ColdTrace.Platform.Reports.Application.Errors;

/// <summary>
///     Errors that can occur while retrieving one report.
/// </summary>
public enum GetReportByIdAndOrganizationError
{
    OrganizationNotFound,
    ReportNotFound,
    UnexpectedError
}
