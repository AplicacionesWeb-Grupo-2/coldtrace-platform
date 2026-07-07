namespace ColdTrace.Platform.Reports.Application.Errors;

/// <summary>
///     Errors that can occur while generating an AI report summary.
/// </summary>
public enum GenerateReportAiSummaryError
{
    OrganizationNotFound,
    ReportNotFound,
    ReportContextUnavailable,
    AiProviderDisabled,
    AiProviderNotConfigured,
    AiProviderUnavailable,
    AiProviderTimeout,
    InvalidStructuredOutput,
    UnexpectedError
}
