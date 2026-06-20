using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.AssetManagement.Domain.Repositories;

/// <summary>
///     Gateway repository contract.
/// </summary>
public interface IGatewayRepository : IBaseRepository<Gateway>
{
    /// <summary>
    ///     Finds all gateways that belong to an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Gateways for the organization.</returns>
    Task<IEnumerable<Gateway>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds one gateway by id and organization.
    /// </summary>
    /// <param name="id">Gateway identifier.</param>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The gateway when found; otherwise null.</returns>
    Task<Gateway?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether a gateway UUID already exists in an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="uuid">Gateway UUID.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when the UUID is already used.</returns>
    Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether another gateway already uses a UUID in an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="uuid">Gateway UUID.</param>
    /// <param name="id">Gateway identifier to exclude.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when another gateway already uses the UUID.</returns>
    Task<bool> ExistsByOrganizationIdAndUuidAndIdNotAsync(
        int organizationId,
        string uuid,
        int id,
        CancellationToken cancellationToken = default);
}
