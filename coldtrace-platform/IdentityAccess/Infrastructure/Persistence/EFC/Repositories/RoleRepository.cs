using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;

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
}
