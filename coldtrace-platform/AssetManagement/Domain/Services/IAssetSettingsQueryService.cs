using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

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
