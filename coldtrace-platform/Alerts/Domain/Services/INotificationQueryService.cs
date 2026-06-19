using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Alerts.Domain.Services;

/// <summary>
///     Application service contract for notification queries.
/// </summary>
public interface INotificationQueryService
{
    Task<Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>> Handle(
        GetNotificationsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Notification>, GetNotificationsByIncidentError>> Handle(
        GetNotificationsByIncidentIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
