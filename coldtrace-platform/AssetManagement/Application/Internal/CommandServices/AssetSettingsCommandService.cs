using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Application.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for asset settings command operations.
/// </summary>
public class AssetSettingsCommandService(
    IAssetSettingsRepository assetSettingsRepository,
    IAssetRepository assetRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<AssetSettingsCommandService> logger)
    : IAssetSettingsCommandService
{
    /// <inheritdoc />
    public async Task<Result<AssetSettings, SaveAssetSettingsError>> Handle(
        SaveAssetSettingsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(
                SaveAssetSettingsError.OrganizationNotFound);

        if (command.AssetId is not null)
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
                return new Result<AssetSettings, SaveAssetSettingsError>.Failure(SaveAssetSettingsError.AssetNotFound);
        }

        try
        {
            var settings = command.AssetId is null
                ? await assetSettingsRepository.FindDefaultByOrganizationIdAsync(command.OrganizationId, cancellationToken)
                : await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                    command.OrganizationId,
                    command.AssetId.Value,
                    cancellationToken);

            if (settings is null)
            {
                settings = new AssetSettings(command);
                await assetSettingsRepository.AddAsync(settings, cancellationToken);
            }
            else
            {
                settings.Update(command);
            }

            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<AssetSettings, SaveAssetSettingsError>.Success(settings);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed saving asset settings for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(SaveAssetSettingsError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error saving asset settings for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<AssetSettings, SaveAssetSettingsError>.Failure(SaveAssetSettingsError.UnexpectedError);
        }
    }
}
