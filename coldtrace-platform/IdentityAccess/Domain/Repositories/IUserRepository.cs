using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Domain.Repositories;

/// <summary>
///     User repository contract.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    ///     Determines whether a user exists with the provided email address.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>True when a user with the same email exists.</returns>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a user by normalized email address.
    /// </summary>
    /// <param name="email">User email address.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user when found; otherwise null.</returns>
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds users by owning organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>Users that belong to the organization.</returns>
    Task<IEnumerable<User>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds a user by id and owning organization.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user when it belongs to the organization; otherwise null.</returns>
    Task<User?> FindByIdAndOrganizationIdAsync(
        int userId,
        int organizationId,
        CancellationToken cancellationToken = default);
}
