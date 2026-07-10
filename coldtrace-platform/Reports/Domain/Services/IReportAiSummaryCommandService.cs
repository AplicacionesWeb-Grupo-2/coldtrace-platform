using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Application.Results;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Reports.Domain.Services;

/// <summary>
///     Application service contract for AI-assisted report summaries.
/// </summary>
public interface IReportAiSummaryCommandService
{
    Task<Result<ReportAiSummary, GenerateReportAiSummaryError>> Handle(
        GenerateReportAiSummaryCommand command,
        CancellationToken cancellationToken = default);
}
