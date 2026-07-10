using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for asset command operations.
/// </summary>
public class AssetCommandService(
    IAssetRepository assetRepository,
    ILocationRepository locationRepository,
    IOrganizationRepository organizationRepository,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<AssetCommandService> logger)
    : IAssetCommandService
{
    /// <inheritdoc />
    public async Task<Result<Asset, CreateAssetError>> Handle(
        CreateAssetCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for asset creation: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.LocationNotFound);
        }

        if (await assetRepository.ExistsByOrganizationIdAndUuidAsync(
                command.OrganizationId,
                command.Uuid,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate asset UUID rejected: {OrganizationId} {Uuid}",
                command.OrganizationId,
                command.Uuid);
            return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.DuplicateUuid);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementAssets,
            "AssetPlanLimitExceeded",
            cancellationToken);

        try
        {
            var asset = new Asset(command);
            await assetRepository.AddAsync(asset, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Asset, CreateAssetError>.Success(asset);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateAssetUuidError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation creating asset {Uuid} for organization {OrganizationId}",
                    command.Uuid,
                    command.OrganizationId);
                return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.DuplicateUuid);
            }

            logger.LogError(
                ex,
                "Database update failed creating asset {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error creating asset {Uuid} for organization {OrganizationId}",
                command.Uuid,
                command.OrganizationId);
            return new Result<Asset, CreateAssetError>.Failure(CreateAssetError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Asset, UpdateAssetError>> Handle(
        UpdateAssetCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset update: {OrganizationId}",
                command.OrganizationId);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.OrganizationNotFound);
        }

        var location = await locationRepository.FindByIdAndOrganizationIdAsync(
            command.LocationId,
            command.OrganizationId,
            cancellationToken);
        if (location is null)
        {
            logger.LogWarning(
                "Location not found for asset update: {OrganizationId} {LocationId}",
                command.OrganizationId,
                command.LocationId);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.LocationNotFound);
        }

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
            command.AssetId,
            command.OrganizationId,
            cancellationToken);
        if (asset is null)
        {
            logger.LogWarning(
                "Asset not found for update: {OrganizationId} {AssetId}",
                command.OrganizationId,
                command.AssetId);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.AssetNotFound);
        }

        if (await assetRepository.ExistsByOrganizationIdAndUuidAndIdNotAsync(
                command.OrganizationId,
                command.Uuid,
                command.AssetId,
                cancellationToken))
        {
            logger.LogWarning(
                "Duplicate asset UUID rejected for update: {OrganizationId} {AssetId} {Uuid}",
                command.OrganizationId,
                command.AssetId,
                command.Uuid);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.DuplicateUuid);
        }

        try
        {
            asset.Update(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<Asset, UpdateAssetError>.Success(asset);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateAssetUuidError(ex))
            {
                logger.LogWarning(
                    ex,
                    "Duplicate key violation updating asset {AssetId} for organization {OrganizationId}",
                    command.AssetId,
                    command.OrganizationId);
                return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.DuplicateUuid);
            }

            logger.LogError(
                ex,
                "Database update failed updating asset {AssetId} for organization {OrganizationId}",
                command.AssetId,
                command.OrganizationId);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error updating asset {AssetId} for organization {OrganizationId}",
                command.AssetId,
                command.OrganizationId);
            return new Result<Asset, UpdateAssetError>.Failure(UpdateAssetError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteAssetCommand, DeleteAssetError>> Handle(
        DeleteAssetCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset deletion: {OrganizationId}",
                command.OrganizationId);
            return new Result<DeleteAssetCommand, DeleteAssetError>.Failure(
                DeleteAssetError.OrganizationNotFound);
        }

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
            command.AssetId,
            command.OrganizationId,
            cancellationToken);
        if (asset is null)
        {
            logger.LogWarning(
                "Asset not found for deletion: {OrganizationId} {AssetId}",
                command.OrganizationId,
                command.AssetId);
            return new Result<DeleteAssetCommand, DeleteAssetError>.Failure(DeleteAssetError.AssetNotFound);
        }

        try
        {
            assetRepository.Remove(asset);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation(
                "Asset deleted: {AssetId} {OrganizationId}",
                command.AssetId,
                command.OrganizationId);
            return new Result<DeleteAssetCommand, DeleteAssetError>.Success(command);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(
                ex,
                "Asset deletion blocked by a database constraint: {OrganizationId} {AssetId}",
                command.OrganizationId,
                command.AssetId);
            return new Result<DeleteAssetCommand, DeleteAssetError>.Failure(DeleteAssetError.DeleteBlocked);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error deleting asset {AssetId} for organization {OrganizationId}",
                command.AssetId,
                command.OrganizationId);
            return new Result<DeleteAssetCommand, DeleteAssetError>.Failure(DeleteAssetError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateAssetUuidError(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062 &&
                current.Message.Contains("i_x_assets_organization_id_uuid", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
