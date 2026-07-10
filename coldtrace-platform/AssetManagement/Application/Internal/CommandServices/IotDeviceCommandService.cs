using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Application.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.QueryServices;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for IoT device command operations.
/// </summary>
public class IotDeviceCommandService(
    IIotDeviceRepository iotDeviceRepository,
    IAssetRepository assetRepository,
    IGatewayRepository gatewayRepository,
    IIamContextFacade iamContextFacade,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<IotDeviceCommandService> logger)
    : IIotDeviceCommandService
{
    /// <inheritdoc />
    public async Task<Result<IotDevice, CreateIotDeviceError>> Handle(
        CreateIotDeviceCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for IoT device creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.OrganizationNotFound);
        }

        var gateway = await gatewayRepository.FindByIdAndOrganizationIdAsync(
            command.GatewayId,
            command.OrganizationId,
            cancellationToken);
        if (gateway is null)
        {
            logger.LogWarning("Gateway not found for IoT device creation: {OrganizationId} {GatewayId}",
                command.OrganizationId,
                command.GatewayId);
            return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.GatewayNotFound);
        }

        if (command.AssetId is not null)
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
            {
                logger.LogWarning("Asset not found for IoT device creation: {OrganizationId} {AssetId}",
                    command.OrganizationId,
                    command.AssetId);
                return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.AssetNotFound);
            }

            if (asset.LocationId != gateway.LocationId)
            {
                logger.LogWarning(
                    "Asset location not compatible with gateway for IoT device creation: {OrganizationId} {AssetId} {GatewayId} {AssetLocationId} {GatewayLocationId}",
                    command.OrganizationId,
                    command.AssetId,
                    command.GatewayId,
                    asset.LocationId,
                    gateway.LocationId);
                return new Result<IotDevice, CreateIotDeviceError>.Failure(
                    CreateIotDeviceError.AssetLocationNotCompatible);
            }
        }

        if (await iotDeviceRepository.ExistsByOrganizationIdAndUuidAsync(
                command.OrganizationId,
                command.Uuid,
                cancellationToken))
        {
            logger.LogWarning("Duplicate IoT device UUID rejected: {OrganizationId} {Uuid}",
                command.OrganizationId,
                command.Uuid);
            return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.DuplicateUuid);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementIotDevices,
            "IotDevicePlanLimitExceeded",
            cancellationToken);

        try
        {
            var iotDevice = new IotDevice(command);
            await iotDeviceRepository.AddAsync(iotDevice, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<IotDevice, CreateIotDeviceError>.Success(iotDevice);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateIotDeviceUuidError(ex))
            {
                logger.LogWarning(ex,
                    "Duplicate key violation creating IoT device {Uuid} for organization {OrganizationId}",
                    command.Uuid,
                    command.OrganizationId);
                return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.DuplicateUuid);
            }

            logger.LogError(ex,
                "Database update failed creating IoT device {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating IoT device {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<IotDevice, CreateIotDeviceError>.Failure(CreateIotDeviceError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IotDevice, UpdateIotDeviceError>> Handle(
        UpdateIotDeviceCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for IoT device update: {OrganizationId}",
                command.OrganizationId);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.OrganizationNotFound);
        }

        var gateway = await gatewayRepository.FindByIdAndOrganizationIdAsync(
            command.GatewayId,
            command.OrganizationId,
            cancellationToken);
        if (gateway is null)
        {
            logger.LogWarning("Gateway not found for IoT device update: {OrganizationId} {GatewayId}",
                command.OrganizationId,
                command.GatewayId);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.GatewayNotFound);
        }

        var iotDevice = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
            command.IotDeviceId,
            command.OrganizationId,
            cancellationToken);
        if (iotDevice is null)
        {
            logger.LogWarning("IoT device not found for update: {OrganizationId} {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.IotDeviceNotFound);
        }

        if (command.AssetId is not null)
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
            {
                logger.LogWarning("Asset not found for IoT device update: {OrganizationId} {AssetId}",
                    command.OrganizationId,
                    command.AssetId);
                return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.AssetNotFound);
            }

            if (asset.LocationId != gateway.LocationId)
            {
                logger.LogWarning(
                    "Asset location not compatible with gateway for IoT device update: {OrganizationId} {AssetId} {GatewayId} {AssetLocationId} {GatewayLocationId}",
                    command.OrganizationId,
                    command.AssetId,
                    command.GatewayId,
                    asset.LocationId,
                    gateway.LocationId);
                return new Result<IotDevice, UpdateIotDeviceError>.Failure(
                    UpdateIotDeviceError.AssetLocationNotCompatible);
            }
        }

        if (await iotDeviceRepository.ExistsByOrganizationIdAndUuidAndIdNotAsync(
                command.OrganizationId,
                command.Uuid,
                command.IotDeviceId,
                cancellationToken))
        {
            logger.LogWarning("Duplicate IoT device UUID rejected for update: {OrganizationId} {IotDeviceId} {Uuid}",
                command.OrganizationId,
                command.IotDeviceId,
                command.Uuid);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.DuplicateUuid);
        }

        try
        {
            iotDevice.Update(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<IotDevice, UpdateIotDeviceError>.Success(iotDevice);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateIotDeviceUuidError(ex))
            {
                logger.LogWarning(ex,
                    "Duplicate key violation updating IoT device {IotDeviceId} for organization {OrganizationId}",
                    command.IotDeviceId,
                    command.OrganizationId);
                return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.DuplicateUuid);
            }

            logger.LogError(ex,
                "Database update failed updating IoT device {IotDeviceId} for organization {OrganizationId}",
                command.IotDeviceId,
                command.OrganizationId);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error updating IoT device {IotDeviceId} for organization {OrganizationId}",
                command.IotDeviceId,
                command.OrganizationId);
            return new Result<IotDevice, UpdateIotDeviceError>.Failure(UpdateIotDeviceError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteIotDeviceCommand, DeleteIotDeviceError>> Handle(
        DeleteIotDeviceCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for IoT device deletion: {OrganizationId}",
                command.OrganizationId);
            return new Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Failure(
                DeleteIotDeviceError.OrganizationNotFound);
        }

        var iotDevice = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
            command.IotDeviceId,
            command.OrganizationId,
            cancellationToken);
        if (iotDevice is null)
        {
            logger.LogWarning("IoT device not found for deletion: {OrganizationId} {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Failure(
                DeleteIotDeviceError.IotDeviceNotFound);
        }

        try
        {
            iotDeviceRepository.Remove(iotDevice);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("IoT device deleted: {IotDeviceId} {OrganizationId}",
                command.IotDeviceId,
                command.OrganizationId);
            return new Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Success(command);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex,
                "IoT device deletion blocked by related records: {OrganizationId} {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Failure(
                DeleteIotDeviceError.DeleteBlocked);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error deleting IoT device {IotDeviceId} for organization {OrganizationId}",
                command.IotDeviceId,
                command.OrganizationId);
            return new Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Failure(
                DeleteIotDeviceError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateIotDeviceUuidError(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062 &&
                current.Message.Contains("i_x_iot_devices_organization_id_uuid", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
