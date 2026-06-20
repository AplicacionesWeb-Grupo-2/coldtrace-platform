namespace ColdTrace.Platform.Shared.Domain.Repositories;

/// <summary>
///     Base repository interface for all repositories.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IBaseRepository<TEntity>
{
    /// <summary>
    ///     Add entity to the repository.
    /// </summary>
    /// <param name="entity">Entity object to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find entity by id.
    /// </summary>
    /// <param name="id">The entity id to find.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous read operation, containing the entity if found, or null otherwise.</returns>
    Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Update entity.
    /// </summary>
    /// <param name="entity">The entity object to update.</param>
    void Update(TEntity entity);

    /// <summary>
    ///     Remove entity.
    /// </summary>
    /// <param name="entity">The entity object to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    ///     Get all entities.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An enumerable containing all entity objects.</returns>
    Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default);
}
