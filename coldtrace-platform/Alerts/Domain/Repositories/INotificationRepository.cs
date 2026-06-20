using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Alerts.Domain.Repositories;

/// <summary>
///     Notification read model repository contract.
/// </summary>
public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IEnumerable<Notification>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Notification>> FindAllByIncidentIdAndOrganizationIdAsync(
        int incidentId,
        int organizationId,
        CancellationToken cancellationToken = default);
}
