using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for user persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return await Context.Set<User>()
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .Where(user => user.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> FindByIdAndOrganizationIdAsync(
        int userId,
        int organizationId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(
                user => user.Id == userId && user.OrganizationId == organizationId,
                cancellationToken);
    }
}
