namespace ColdTrace.Platform.Reports.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while retrieving one report.
/// </summary>
public enum GetReportByIdAndOrganizationError
{
    OrganizationNotFound,
    ReportNotFound,
    UnexpectedError
}
