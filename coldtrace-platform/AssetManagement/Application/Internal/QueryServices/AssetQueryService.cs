using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;

/// <summary>
///     Application service for asset query operations.
/// </summary>
public class AssetQueryService(
    IAssetRepository assetRepository,
    IOrganizationRepository organizationRepository,
    ILogger<AssetQueryService> logger)
    : IAssetQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Asset>, GetAssetsByOrganizationError>> Handle(
        GetAssetsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for asset query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Asset>, GetAssetsByOrganizationError>.Failure(
                GetAssetsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var assets = await assetRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Asset>, GetAssetsByOrganizationError>.Success(assets);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying assets for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Asset>, GetAssetsByOrganizationError>.Failure(
                GetAssetsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Asset, GetAssetByIdAndOrganizationError>> Handle(
        GetAssetByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for asset by id query: {OrganizationId}",
                query.OrganizationId);
            return new Result<Asset, GetAssetByIdAndOrganizationError>.Failure(
                GetAssetByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                query.AssetId,
                query.OrganizationId,
                cancellationToken);
            if (asset is null)
            {
                logger.LogWarning(
                    "Asset not found for organization query: {OrganizationId} {AssetId}",
                    query.OrganizationId,
                    query.AssetId);
                return new Result<Asset, GetAssetByIdAndOrganizationError>.Failure(
                    GetAssetByIdAndOrganizationError.AssetNotFound);
            }

            return new Result<Asset, GetAssetByIdAndOrganizationError>.Success(asset);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error querying asset {AssetId} for organization {OrganizationId}",
                query.AssetId,
                query.OrganizationId);
            return new Result<Asset, GetAssetByIdAndOrganizationError>.Failure(
                GetAssetByIdAndOrganizationError.UnexpectedError);
        }
    }
}