using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.AssetManagement.Domain.Repositories;

/// <summary>
///     Location repository contract.
/// </summary>
public interface ILocationRepository : IBaseRepository<Location>
{
    /// <summary>
    ///     Finds all locations that belong to an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Locations for the organization.</returns>
    Task<IEnumerable<Location>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds one location by id and organization.
    /// </summary>
    /// <param name="id">Location identifier.</param>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The location when found; otherwise null.</returns>
    Task<Location?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether a location name already exists in an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="name">Location name.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when the name is already used.</returns>
    Task<bool> ExistsByOrganizationIdAndNameAsync(
        int organizationId,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether another location already uses a name in an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="name">Location name.</param>
    /// <param name="id">Location identifier to exclude.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when another location already uses the name.</returns>
    Task<bool> ExistsByOrganizationIdAndNameAndIdNotAsync(
        int organizationId,
        string name,
        int id,
        CancellationToken cancellationToken = default);
}
