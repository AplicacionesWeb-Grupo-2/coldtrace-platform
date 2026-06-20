using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Alerts.Application.Internal.CommandServices;

/// <summary>
///     Application service for incident command operations.
/// </summary>
public class IncidentCommandService(
    IIncidentRepository incidentRepository,
    INotificationRepository notificationRepository,
    IOrganizationRepository organizationRepository,
    IAssetRepository assetRepository,
    IUnitOfWork unitOfWork,
    ILogger<IncidentCommandService> logger)
    : IIncidentCommandService
{
    /// <inheritdoc />
    public async Task<Result<Incident, CreateIncidentError>> Handle(
        CreateIncidentCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for incident creation: {OrganizationId}", command.OrganizationId);
            return new Result<Incident, CreateIncidentError>.Failure(CreateIncidentError.OrganizationNotFound);
        }

        Asset? asset = null;
        if (command.AssetId is not null)
        {
            asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
            {
                logger.LogWarning(
                    "Asset not found for incident creation: {OrganizationId} {AssetId}",
                    command.OrganizationId,
                    command.AssetId);
                return new Result<Incident, CreateIncidentError>.Failure(CreateIncidentError.AssetNotFound);
            }
        }

        try
        {
            var normalizedCommand = new CreateIncidentCommand(
                command.OrganizationId,
                command.AssetId,
                command.DeviceId,
                command.ReadingId,
                command.AssetName ?? asset?.Name,
                command.DeviceName,
                command.Type,
                command.Severity,
                command.Value);
            var incident = new Incident(normalizedCommand);

            await incidentRepository.AddAsync(incident, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await EmitNotificationAsync(incident, Notification.IncidentOpened(incident), cancellationToken);

            return new Result<Incident, CreateIncidentError>.Success(incident);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed creating incident for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<Incident, CreateIncidentError>.Failure(CreateIncidentError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error creating incident for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<Incident, CreateIncidentError>.Failure(CreateIncidentError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Incident, AcknowledgeIncidentError>> Handle(
        AcknowledgeIncidentCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for incident acknowledgement: {OrganizationId}",
                command.OrganizationId);
            return new Result<Incident, AcknowledgeIncidentError>.Failure(
                AcknowledgeIncidentError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for acknowledgement: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return new Result<Incident, AcknowledgeIncidentError>.Failure(AcknowledgeIncidentError.IncidentNotFound);
        }

        if (incident.IsResolved())
            return new Result<Incident, AcknowledgeIncidentError>.Failure(AcknowledgeIncidentError.AlreadyResolved);

        if (incident.IsAcknowledged())
            return new Result<Incident, AcknowledgeIncidentError>.Failure(AcknowledgeIncidentError.AlreadyAcknowledged);

        try
        {
            incident.Acknowledge(command);
            await EmitNotificationAsync(incident, Notification.IncidentAcknowledged(incident), cancellationToken);

            return new Result<Incident, AcknowledgeIncidentError>.Success(incident);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed acknowledging incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, AcknowledgeIncidentError>.Failure(AcknowledgeIncidentError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error acknowledging incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, AcknowledgeIncidentError>.Failure(AcknowledgeIncidentError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Incident, ResolveIncidentError>> Handle(
        ResolveIncidentCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for incident resolution: {OrganizationId}", command.OrganizationId);
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for resolution: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.IncidentNotFound);
        }

        if (incident.IsResolved())
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.AlreadyResolved);

        if (!incident.IsOpen() && !incident.IsAcknowledged())
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.InvalidLifecycleTransition);

        try
        {
            incident.Resolve(command);
            await EmitNotificationAsync(incident, Notification.IncidentResolved(incident), cancellationToken);

            return new Result<Incident, ResolveIncidentError>.Success(incident);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed resolving incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error resolving incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, ResolveIncidentError>.Failure(ResolveIncidentError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Incident, EscalateIncidentError>> Handle(
        EscalateIncidentCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for incident escalation: {OrganizationId}", command.OrganizationId);
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for escalation: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.IncidentNotFound);
        }

        if (incident.IsResolved())
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.AlreadyResolved);

        if (incident.IsEscalated())
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.AlreadyEscalated);

        try
        {
            incident.Escalate(command);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<Incident, EscalateIncidentError>.Success(incident);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed escalating incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error escalating incident {IncidentId} for organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, EscalateIncidentError>.Failure(EscalateIncidentError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Incident, RegisterIncidentCorrectiveActionError>> Handle(
        RegisterIncidentCorrectiveActionCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for incident corrective action: {OrganizationId}",
                command.OrganizationId);
            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Failure(
                RegisterIncidentCorrectiveActionError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            command.IncidentId,
            command.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for corrective action: {OrganizationId} {IncidentId}",
                command.OrganizationId,
                command.IncidentId);
            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Failure(
                RegisterIncidentCorrectiveActionError.IncidentNotFound);
        }

        if (incident.IsResolved())
            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Failure(
                RegisterIncidentCorrectiveActionError.AlreadyResolved);

        try
        {
            incident.RegisterCorrectiveAction(command);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Success(incident);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed registering corrective action for incident {IncidentId} in organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Failure(
                RegisterIncidentCorrectiveActionError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error registering corrective action for incident {IncidentId} in organization {OrganizationId}",
                command.IncidentId,
                command.OrganizationId);
            return new Result<Incident, RegisterIncidentCorrectiveActionError>.Failure(
                RegisterIncidentCorrectiveActionError.UnexpectedError);
        }
    }

    private async Task EmitNotificationAsync(
        Incident incident,
        Notification notification,
        CancellationToken cancellationToken)
    {
        await notificationRepository.AddAsync(notification, cancellationToken);
        incident.RecordNotification(notification.Status);
        await unitOfWork.CompleteAsync(cancellationToken);
    }
}
