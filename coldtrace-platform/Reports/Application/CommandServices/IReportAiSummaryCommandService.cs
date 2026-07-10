using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Application.Results;
using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Reports.Application.CommandServices;

/// <summary>
///     Application service contract for AI-assisted report summaries.
/// </summary>
public interface IReportAiSummaryCommandService
{
    Task<Result<ReportAiSummary, GenerateReportAiSummaryError>> Handle(
        GenerateReportAiSummaryCommand command,
        CancellationToken cancellationToken = default);
}
