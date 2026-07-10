namespace ColdTrace.Platform.Reports.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while generating a report.
/// </summary>
public enum GenerateReportError
{
    OrganizationNotFound,
    UnexpectedError
}
