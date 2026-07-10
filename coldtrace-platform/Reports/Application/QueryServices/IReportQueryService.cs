using ColdTrace.Platform.Reports.Domain.Model.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Reports.Application.QueryServices;

/// <summary>
///     Application service contract for report queries.
/// </summary>
public interface IReportQueryService
{
    Task<Result<IEnumerable<Report>, GetReportsByOrganizationError>> Handle(
        GetReportsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<Report, GetReportByIdAndOrganizationError>> Handle(
        GetReportByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
