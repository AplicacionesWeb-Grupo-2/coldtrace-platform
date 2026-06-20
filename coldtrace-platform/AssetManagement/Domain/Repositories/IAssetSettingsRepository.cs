using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.AssetManagement.Domain.Repositories;

/// <summary>
///     Asset settings repository contract.
/// </summary>
public interface IAssetSettingsRepository : IBaseRepository<AssetSettings>
{
    Task<IEnumerable<AssetSettings>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<AssetSettings?> FindByOrganizationIdAndAssetIdAsync(
        int organizationId,
        int assetId,
        CancellationToken cancellationToken = default);

    Task<AssetSettings?> FindDefaultByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);
}
