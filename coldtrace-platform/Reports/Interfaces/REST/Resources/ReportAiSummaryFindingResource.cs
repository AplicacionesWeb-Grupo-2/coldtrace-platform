using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Resources;

/// <summary>
///     Response resource representing one AI-generated report finding.
/// </summary>
[SwaggerSchema(Description = "One AI-generated report finding")]
public record ReportAiSummaryFindingResource(
    [SwaggerParameter(Description = "Compliance or operational area")]
    string Area,
    [SwaggerParameter(Description = "Finding status")]
    string Status,
    [SwaggerParameter(Description = "Factual evidence summarized from report context")]
    string Evidence,
    [SwaggerParameter(Description = "Advisory recommendation")]
    string Recommendation);
