namespace ColdTrace.Platform.Shared.Domain.Repositories;

/// <summary>
///     Unit of work interface.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Commit changes to the database.
    /// </summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
