using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Domain.Repositories;

/// <summary>
///     Role repository contract.
/// </summary>
public interface IRoleRepository : IBaseRepository<Role>
{
    /// <summary>
    ///     Finds a role by stable role name.
    /// </summary>
    /// <param name="name">Stable role name.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The role when found; otherwise null.</returns>
    Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists roles with their permission metadata.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Roles with permissions.</returns>
    Task<IEnumerable<Role>> ListWithPermissionsAsync(CancellationToken cancellationToken = default);
}
