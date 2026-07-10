using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Application.CommandServices;
using ColdTrace.Platform.Alerts.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Alerts.Application.Internal.QueryServices;

/// <summary>
///     Application service for notification read model queries.
/// </summary>
public class NotificationQueryService(
    INotificationRepository notificationRepository,
    IIncidentRepository incidentRepository,
    IIamContextFacade iamContextFacade,
    ILogger<NotificationQueryService> logger)
    : INotificationQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>> Handle(
        GetNotificationsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for notification query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>.Failure(
                GetNotificationsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var notifications = await notificationRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>.Success(notifications);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting notifications for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Notification>, GetNotificationsByOrganizationError>.Failure(
                GetNotificationsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<Notification>, GetNotificationsByIncidentError>> Handle(
        GetNotificationsByIncidentIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for incident notification query: {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Failure(
                GetNotificationsByIncidentError.OrganizationNotFound);
        }

        if (!await incidentRepository.ExistsByIdAndOrganizationIdAsync(
                query.IncidentId,
                query.OrganizationId,
                cancellationToken))
        {
            logger.LogWarning(
                "Incident not found for notification query: {OrganizationId} {IncidentId}",
                query.OrganizationId,
                query.IncidentId);
            return new Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Failure(
                GetNotificationsByIncidentError.IncidentNotFound);
        }

        try
        {
            var notifications = await notificationRepository.FindAllByIncidentIdAndOrganizationIdAsync(
                query.IncidentId,
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Success(notifications);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error getting notifications for organization {OrganizationId} and incident {IncidentId}",
                query.OrganizationId,
                query.IncidentId);
            return new Result<IEnumerable<Notification>, GetNotificationsByIncidentError>.Failure(
                GetNotificationsByIncidentError.UnexpectedError);
        }
    }
}
