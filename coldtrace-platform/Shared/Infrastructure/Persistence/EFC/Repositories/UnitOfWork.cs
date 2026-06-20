using ColdTrace.Platform.Shared.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Unit of work implementation for coordinating database transactions.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
