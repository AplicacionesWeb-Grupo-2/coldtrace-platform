using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.AssetManagement.Application.QueryServices;

/// <summary>
///     Application service contract for asset settings queries.
/// </summary>
public interface IAssetSettingsQueryService
{
    Task<IEnumerable<AssetSettings>> Handle(
        GetAssetSettingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<AssetSettings, GetEffectiveAssetSettingsError>> Handle(
        GetEffectiveAssetSettingsByAssetIdQuery query,
        CancellationToken cancellationToken = default);
}
