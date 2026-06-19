using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Asset settings query application service contract.
/// </summary>
public interface IAssetSettingsQueryService
{
    Task<Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>> Handle(
        GetAssetSettingsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>> Handle(
        GetEffectiveAssetSettingsByAssetIdQuery query,
        CancellationToken cancellationToken = default);
}