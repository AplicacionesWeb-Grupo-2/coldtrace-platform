namespace ColdTrace.Platform.Reports.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one generated report inside an organization.
/// </summary>
public record GetReportByIdAndOrganizationIdQuery(int OrganizationId, int ReportId);
