using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Interfaces.REST.Resources;

namespace ColdTrace.Platform.Reports.Interfaces.REST.Transform;

/// <summary>
///     Assembles report resources from aggregates.
/// </summary>
public static class ReportResourceFromEntityAssembler
{
    public static ReportResource ToResourceFromEntity(Report report) =>
        new(
            report.Id,
            report.OrganizationId,
            report.Uuid,
            report.Type,
            report.Title,
            PeriodLabel(report),
            report.PeriodStart,
            report.PeriodEnd,
            report.GeneratedAt,
            report.AssetCount,
            report.ReadingCount,
            report.OutOfRangeReadingCount,
            report.IncidentCount,
            report.OpenIncidentCount,
            report.AverageTemperature,
            report.AverageHumidity,
            report.CompliancePercentage);

    private static string PeriodLabel(Report report) =>
        $"{report.PeriodStart:yyyy-MM-dd}/{report.PeriodEnd:yyyy-MM-dd}";
}
