using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing an advisory AI summary for a persisted report.
/// </summary>
[SwaggerSchema(Description = "An advisory AI summary for a persisted report")]
public record ReportAiSummaryResource(
    [SwaggerParameter(Description = "Organization that owns the source report")]
    int OrganizationId,
    [SwaggerParameter(Description = "Source report identifier")]
    int ReportId,
    [SwaggerParameter(Description = "Source report business identifier")]
    string ReportUuid,
    [SwaggerParameter(Description = "Source report type")]
    string ReportType,
    [SwaggerParameter(Description = "Source report title")]
    string ReportTitle,
    [SwaggerParameter(Description = "AI summary generation timestamp")]
    DateTimeOffset SummaryGeneratedAt,
    [SwaggerParameter(Description = "Factual report metrics used as source of truth")]
    ReportResource SourceReport,
    [SwaggerParameter(Description = "Concise advisory summary")]
    string ExecutiveSummary,
    [SwaggerParameter(Description = "Structured report findings")]
    IReadOnlyCollection<ReportAiSummaryFindingResource> Findings,
    [SwaggerParameter(Description = "Missing evidence that limits certainty")]
    IReadOnlyCollection<string> EvidenceGaps,
    [SwaggerParameter(Description = "Advisory next actions")]
    IReadOnlyCollection<string> RecommendedActions,
    [SwaggerParameter(Description = "Assumptions or missing-context notes")]
    IReadOnlyCollection<string> UncertaintyNotes,
    [SwaggerParameter(Description = "Configured AI provider used for this response")]
    string ModelProvider,
    [SwaggerParameter(Description = "Configured AI model used for this response")]
    string ModelName);
