using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for role persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class RoleRepository(AppDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    /// <inheritdoc />
    public async Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await Context.Set<Role>()
            .Include(role => role.Permissions)
            .FirstOrDefaultAsync(role => role.Name == normalizedName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Role>> ListWithPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Role>()
            .Include(role => role.Permissions)
            .OrderBy(role => role.Id)
            .ToListAsync(cancellationToken);
    }
}
