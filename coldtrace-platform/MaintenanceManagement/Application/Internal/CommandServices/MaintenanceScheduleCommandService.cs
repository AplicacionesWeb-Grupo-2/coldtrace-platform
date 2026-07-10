using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.CommandServices;
using ColdTrace.Platform.MaintenanceManagement.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.MaintenanceManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for maintenance schedule command operations.
/// </summary>
public class MaintenanceScheduleCommandService(
    IMaintenanceScheduleRepository maintenanceScheduleRepository,
    IIamContextFacade iamContextFacade,
    IAssetRepository assetRepository,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<MaintenanceScheduleCommandService> logger)
    : IMaintenanceScheduleCommandService
{
    /// <inheritdoc />
    public async Task<Result<MaintenanceSchedule, CreateMaintenanceScheduleError>> Handle(
        CreateMaintenanceScheduleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for maintenance schedule creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Failure(
                CreateMaintenanceScheduleError.OrganizationNotFound);
        }

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
            command.AssetId, command.OrganizationId, cancellationToken);
        if (asset is null)
        {
            logger.LogWarning(
                "Asset not found for maintenance schedule creation: {OrganizationId} {AssetId}",
                command.OrganizationId, command.AssetId);
            return new Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Failure(
                CreateMaintenanceScheduleError.AssetNotFound);
        }

        if (await maintenanceScheduleRepository.HasActiveScheduleForAssetAsync(
                command.OrganizationId, command.AssetId, cancellationToken))
        {
            logger.LogWarning(
                "Duplicate active maintenance schedule rejected: {OrganizationId} {AssetId}",
                command.OrganizationId, command.AssetId);
            return new Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Failure(
                CreateMaintenanceScheduleError.DuplicateActiveSchedule);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementMaintenance,
            "MaintenanceSchedulePlanLimitExceeded",
            cancellationToken);

        try
        {
            var schedule = new MaintenanceSchedule(command);
            await maintenanceScheduleRepository.AddAsync(schedule, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation(
                "Maintenance schedule created: {Id} org={OrganizationId} asset={AssetId} status={Status}",
                schedule.Id, schedule.OrganizationId, schedule.AssetId, schedule.Status);
            return new Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Success(schedule);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating maintenance schedule for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<MaintenanceSchedule, CreateMaintenanceScheduleError>.Failure(
                CreateMaintenanceScheduleError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>> Handle(
        UpdateMaintenanceScheduleStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for maintenance schedule status update: {OrganizationId}",
                command.OrganizationId);
            return new Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Failure(
                UpdateMaintenanceScheduleStatusError.OrganizationNotFound);
        }

        var schedule = await maintenanceScheduleRepository.FindByIdAndOrganizationIdAsync(
            command.MaintenanceScheduleId, command.OrganizationId, cancellationToken);
        if (schedule is null)
        {
            logger.LogWarning(
                "Maintenance schedule not found for status update: {OrganizationId} {MaintenanceScheduleId}",
                command.OrganizationId, command.MaintenanceScheduleId);
            return new Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Failure(
                UpdateMaintenanceScheduleStatusError.MaintenanceScheduleNotFound);
        }

        if (!schedule.CanTransitionTo(command.Status))
        {
            logger.LogWarning(
                "Invalid status transition for maintenance schedule {Id}: {From} -> {To}",
                schedule.Id, schedule.Status, command.Status);
            return new Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Failure(
                UpdateMaintenanceScheduleStatusError.InvalidStatusTransition);
        }

        try
        {
            schedule.UpdateStatus(command.Status);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Maintenance schedule {Id} status updated to {Status}", schedule.Id, schedule.Status);
            return new Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Success(schedule);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating maintenance schedule {Id} status", command.MaintenanceScheduleId);
            return new Result<MaintenanceSchedule, UpdateMaintenanceScheduleStatusError>.Failure(
                UpdateMaintenanceScheduleStatusError.UnexpectedError);
        }
    }
}
