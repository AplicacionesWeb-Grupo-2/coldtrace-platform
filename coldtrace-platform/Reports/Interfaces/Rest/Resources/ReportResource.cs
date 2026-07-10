using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Reports.Interfaces.Rest.Resources;

/// <summary>
///     Response resource representing a generated report.
/// </summary>
[SwaggerSchema(Description = "A generated report resource")]
public record ReportResource(
    [SwaggerParameter(Description = "Report identifier")]
    int Id,
    [SwaggerParameter(Description = "Organization identifier")]
    int OrganizationId,
    [SwaggerParameter(Description = "Report business identifier")]
    string Uuid,
    [SwaggerParameter(Description = "Report type")]
    string Type,
    [SwaggerParameter(Description = "Report title")]
    string Title,
    [SwaggerParameter(Description = "Compatibility period label")]
    string PeriodDate,
    [SwaggerParameter(Description = "Inclusive lower date-time bound")]
    DateTimeOffset PeriodStart,
    [SwaggerParameter(Description = "Inclusive upper date-time bound")]
    DateTimeOffset PeriodEnd,
    [SwaggerParameter(Description = "Generation timestamp")]
    DateTimeOffset GeneratedAt,
    [SwaggerParameter(Description = "Assets considered")]
    int AssetCount,
    [SwaggerParameter(Description = "Readings considered")]
    int ReadingCount,
    [SwaggerParameter(Description = "Out-of-range readings")]
    int OutOfRangeReadingCount,
    [SwaggerParameter(Description = "Incidents considered")]
    int IncidentCount,
    [SwaggerParameter(Description = "Unresolved incidents")]
    int OpenIncidentCount,
    [SwaggerParameter(Description = "Average temperature")]
    double? AverageTemperature,
    [SwaggerParameter(Description = "Average humidity")]
    double? AverageHumidity,
    [SwaggerParameter(Description = "Compliance percentage")]
    double? CompliancePercentage);
