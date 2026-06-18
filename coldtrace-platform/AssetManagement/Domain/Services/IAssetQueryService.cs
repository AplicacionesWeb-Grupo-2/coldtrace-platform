using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Asset query application service contract.
/// </summary>
public interface IAssetQueryService
{
    Task<Result<IEnumerable<Asset>, GetAssetsByOrganizationError>> Handle(
        GetAssetsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<Asset, GetAssetByIdAndOrganizationError>> Handle(
        GetAssetByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}