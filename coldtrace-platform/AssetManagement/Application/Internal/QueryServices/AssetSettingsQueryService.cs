using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for asset settings query operations.
/// </summary>
public class AssetSettingsQueryService(
    IAssetSettingsRepository assetSettingsRepository,
    IAssetRepository assetRepository,
    IOrganizationRepository organizationRepository,
    ILogger<AssetSettingsQueryService> logger)
    : IAssetSettingsQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>> Handle(
        GetAssetSettingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset settings query: {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Failure(
                GetAssetSettingsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var settings = await assetSettingsRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Success(settings);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error querying asset settings for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Failure(
                GetAssetSettingsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>> Handle(
        GetEffectiveAssetSettingsByAssetIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for effective asset settings query: {OrganizationId}",
                query.OrganizationId);
            return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure(
                GetEffectiveAssetSettingsByAssetError.OrganizationNotFound);
        }

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
            query.AssetId,
            query.OrganizationId,
            cancellationToken);
        if (asset is null)
        {
            logger.LogWarning(
                "Asset not found for effective asset settings query: {OrganizationId} {AssetId}",
                query.OrganizationId,
                query.AssetId);
            return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure(
                GetEffectiveAssetSettingsByAssetError.AssetNotFound);
        }

        try
        {
            var settings = await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                query.OrganizationId,
                query.AssetId,
                cancellationToken);

            if (settings is not null)
                return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Success(settings);

            var defaultSettings = await assetSettingsRepository.FindDefaultByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);

            if (defaultSettings is not null)
                return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Success(defaultSettings);

            logger.LogWarning(
                "No effective settings found for asset {AssetId} in organization {OrganizationId}",
                query.AssetId,
                query.OrganizationId);
            return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure(
                GetEffectiveAssetSettingsByAssetError.AssetNotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error querying effective asset settings for asset {AssetId} in organization {OrganizationId}",
                query.AssetId,
                query.OrganizationId);
            return new Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure(
                GetEffectiveAssetSettingsByAssetError.UnexpectedError);
        }
    }
}