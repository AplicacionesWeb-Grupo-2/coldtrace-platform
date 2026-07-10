using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Application.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for asset settings query operations.
/// </summary>
public class AssetSettingsQueryService(
    IAssetSettingsRepository assetSettingsRepository,
    IAssetRepository assetRepository,
    ILogger<AssetSettingsQueryService> logger)
    : IAssetSettingsQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<AssetSettings>> Handle(
        GetAssetSettingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await assetSettingsRepository.FindAllByOrganizationIdAsync(
            query.OrganizationId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<AssetSettings, GetEffectiveAssetSettingsError>> Handle(
        GetEffectiveAssetSettingsByAssetIdQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                query.AssetId,
                query.OrganizationId,
                cancellationToken);
            if (asset is null)
                return new Result<AssetSettings, GetEffectiveAssetSettingsError>.Failure(
                    GetEffectiveAssetSettingsError.AssetNotFound);

            var settings = await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                               query.OrganizationId,
                               query.AssetId,
                               cancellationToken)
                           ?? await assetSettingsRepository.FindDefaultByOrganizationIdAsync(
                               query.OrganizationId,
                               cancellationToken);

            return settings is null
                ? new Result<AssetSettings, GetEffectiveAssetSettingsError>.Failure(
                    GetEffectiveAssetSettingsError.AssetSettingsNotFound)
                : new Result<AssetSettings, GetEffectiveAssetSettingsError>.Success(settings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error resolving asset settings for organization {OrganizationId} and asset {AssetId}",
                query.OrganizationId,
                query.AssetId);
            return new Result<AssetSettings, GetEffectiveAssetSettingsError>.Failure(
                GetEffectiveAssetSettingsError.UnexpectedError);
        }
    }
}
