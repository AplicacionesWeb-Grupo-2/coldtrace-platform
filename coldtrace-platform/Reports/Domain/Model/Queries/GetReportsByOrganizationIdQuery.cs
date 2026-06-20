namespace ColdTrace.Platform.Reports.Domain.Model.Queries;

/// <summary>
///     Query for retrieving generated reports owned by an organization.
/// </summary>
public record GetReportsByOrganizationIdQuery(int OrganizationId);
