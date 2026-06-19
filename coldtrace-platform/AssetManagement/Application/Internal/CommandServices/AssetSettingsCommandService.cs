using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for asset settings command operations.
/// </summary>
public class AssetSettingsCommandService(
    IAssetSettingsRepository assetSettingsRepository,
    IAssetRepository assetRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssetSettingsCommandService> logger)
    : IAssetSettingsCommandService
{
    /// <inheritdoc />
    public async Task<Result<AssetSettings, SaveAssetSettingsError>> Handle(
        SaveAssetSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset settings save: {OrganizationId}",
                command.OrganizationId);
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(
                SaveAssetSettingsError.OrganizationNotFound);
        }

        if (command.AssetId.HasValue)
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
            {
                logger.LogWarning(
                    "Asset not found for asset settings save: {OrganizationId} {AssetId}",
                    command.OrganizationId,
                    command.AssetId);
                return new Result<AssetSettings, SaveAssetSettingsError>.Failure(
                    SaveAssetSettingsError.AssetNotFound);
            }
        }

        try
        {
            AssetSettings settings;

            if (command.AssetId.HasValue)
            {
                var existing = await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                    command.OrganizationId,
                    command.AssetId.Value,
                    cancellationToken);

                if (existing is not null)
                {
                    existing.Update(command);
                    await unitOfWork.CompleteAsync(cancellationToken);
                    return new Result<AssetSettings, SaveAssetSettingsError>.Success(existing);
                }
            }
            else
            {
                var existing = await assetSettingsRepository.FindDefaultByOrganizationIdAsync(
                    command.OrganizationId,
                    cancellationToken);

                if (existing is not null)
                {
                    existing.Update(command);
                    await unitOfWork.CompleteAsync(cancellationToken);
                    return new Result<AssetSettings, SaveAssetSettingsError>.Success(existing);
                }
            }

            settings = new AssetSettings(command);
            await assetSettingsRepository.AddAsync(settings, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<AssetSettings, SaveAssetSettingsError>.Success(settings);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed saving asset settings for organization {OrganizationId} asset {AssetId}",
                command.OrganizationId,
                command.AssetId);
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(
                SaveAssetSettingsError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error saving asset settings for organization {OrganizationId} asset {AssetId}",
                command.OrganizationId,
                command.AssetId);
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(
                SaveAssetSettingsError.UnexpectedError);
        }
    }
}