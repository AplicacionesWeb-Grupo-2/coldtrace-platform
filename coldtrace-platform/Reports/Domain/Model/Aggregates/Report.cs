using ColdTrace.Platform.Reports.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.Reports.Domain.Model.Aggregates;

/// <summary>
///     Generated operational report aggregate.
/// </summary>
public class Report : IAuditableEntity
{
    protected Report()
    {
        Uuid = string.Empty;
        Type = string.Empty;
        Title = string.Empty;
    }

    /// <summary>
    ///     Creates a persisted report snapshot from computed operational metrics.
    /// </summary>
    public Report(
        GenerateReportCommand command,
        int assetCount,
        int readingCount,
        int outOfRangeReadingCount,
        int incidentCount,
        int openIncidentCount,
        double? averageTemperature,
        double? averageHumidity,
        double? compliancePercentage)
    {
        OrganizationId = command.OrganizationId;
        Uuid = "RPT-" + Guid.NewGuid().ToString("N").ToUpperInvariant();
        Type = command.Type;
        Title = command.Title;
        PeriodStart = command.PeriodStart;
        PeriodEnd = command.PeriodEnd;
        GeneratedAt = DateTimeOffset.UtcNow;
        AssetCount = assetCount;
        ReadingCount = readingCount;
        OutOfRangeReadingCount = outOfRangeReadingCount;
        IncidentCount = incidentCount;
        OpenIncidentCount = openIncidentCount;
        AverageTemperature = averageTemperature;
        AverageHumidity = averageHumidity;
        CompliancePercentage = compliancePercentage;
    }

    public int Id { get; private set; }

    public int OrganizationId { get; private set; }

    public string Uuid { get; private set; }

    public string Type { get; private set; }

    public string Title { get; private set; }

    public DateTimeOffset PeriodStart { get; private set; }

    public DateTimeOffset PeriodEnd { get; private set; }

    public DateTimeOffset GeneratedAt { get; private set; }

    public int AssetCount { get; private set; }

    public int ReadingCount { get; private set; }

    public int OutOfRangeReadingCount { get; private set; }

    public int IncidentCount { get; private set; }

    public int OpenIncidentCount { get; private set; }

    public double? AverageTemperature { get; private set; }

    public double? AverageHumidity { get; private set; }

    public double? CompliancePercentage { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
