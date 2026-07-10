using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Resources;

/// <summary>
///     Request resource used to generate an operational report.
/// </summary>
[SwaggerSchema(Description = "Request payload for generating a report")]
public record GenerateReportResource(
    [SwaggerParameter(Description = "Report type")]
    string Type,
    [SwaggerParameter(Description = "Human-readable report title")]
    string Title,
    [SwaggerParameter(Description = "Inclusive lower date-time bound")]
    DateTimeOffset PeriodStart,
    [SwaggerParameter(Description = "Inclusive upper date-time bound")]
    DateTimeOffset PeriodEnd);
