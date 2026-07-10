using ColdTrace.Platform.AiAssistance.Application.StructuredOutputs;
using ColdTrace.Platform.Reports.Application.Results;
using ColdTrace.Platform.Reports.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Transform;

/// <summary>
///     Assembles report AI summary resources from application results.
/// </summary>
public static class ReportAiSummaryResourceFromResultAssembler
{
    public static ReportAiSummaryResource ToResourceFromResult(ReportAiSummary result)
    {
        var report = result.Report;
        var summary = result.Summary;
        return new ReportAiSummaryResource(
            report.OrganizationId,
            report.Id,
            report.Uuid,
            report.Type,
            report.Title,
            result.SummaryGeneratedAt,
            ReportResourceFromEntityAssembler.ToResourceFromEntity(report),
            summary.ExecutiveSummary,
            summary.Findings.Select(ToFindingResource).ToList(),
            summary.EvidenceGaps.ToList(),
            summary.RecommendedActions.ToList(),
            summary.UncertaintyNotes.ToList(),
            result.ModelProvider,
            result.ModelName);
    }

    private static ReportAiSummaryFindingResource ToFindingResource(ComplianceFindingOutput finding) =>
        new(
            finding.Area,
            finding.Status,
            finding.Evidence,
            finding.Recommendation);
}
