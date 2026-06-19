using ColdTrace.Platform.Reports.Application.Errors;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Reports.Domain.Services;

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
