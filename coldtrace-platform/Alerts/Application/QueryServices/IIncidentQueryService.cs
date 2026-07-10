using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Alerts.Application.QueryServices;

/// <summary>
///     Application service contract for incident queries.
/// </summary>
public interface IIncidentQueryService
{
    Task<Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>> Handle(
        GetIncidentsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<Incident, GetIncidentByIdAndOrganizationError>> Handle(
        GetIncidentByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
