using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.QueryServices;

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