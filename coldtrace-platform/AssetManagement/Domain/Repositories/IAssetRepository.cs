using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.AssetManagement.Domain.Repositories;

/// <summary>
///     Asset repository contract.
/// </summary>
public interface IAssetRepository : IBaseRepository<Asset>
{
    Task<IEnumerable<Asset>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<Asset?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByOrganizationIdAndUuidAndIdNotAsync(
        int organizationId,
        string uuid,
        int id,
        CancellationToken cancellationToken = default);
}