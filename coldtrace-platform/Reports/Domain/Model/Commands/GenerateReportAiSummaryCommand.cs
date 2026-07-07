namespace ColdTrace.Platform.Reports.Domain.Model.Commands;

/// <summary>
///     Command for generating an advisory AI summary for a persisted report.
/// </summary>
public record GenerateReportAiSummaryCommand
{
    public GenerateReportAiSummaryCommand(int organizationId, int reportId)
    {
        if (organizationId <= 0)
            throw new ArgumentException("Organization identifier must be positive.", nameof(organizationId));
        if (reportId <= 0)
            throw new ArgumentException("Report identifier must be positive.", nameof(reportId));

        OrganizationId = organizationId;
        ReportId = reportId;
    }

    public int OrganizationId { get; }

    public int ReportId { get; }
}
