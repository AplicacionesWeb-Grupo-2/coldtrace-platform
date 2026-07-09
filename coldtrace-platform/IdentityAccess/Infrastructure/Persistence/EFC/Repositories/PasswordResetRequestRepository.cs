using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for password reset request metadata.
/// </summary>
/// <param name="context">Application database context.</param>
public class PasswordResetRequestRepository(AppDbContext context)
    : BaseRepository<PasswordResetRequest>(context), IPasswordResetRequestRepository
{
    /// <inheritdoc />
    public async Task<PasswordResetRequest?> FindByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        var normalizedHash = tokenHash.Trim().ToLowerInvariant();
        return await Context.Set<PasswordResetRequest>()
            .FirstOrDefaultAsync(request => request.TokenHash == normalizedHash, cancellationToken);
    }
}
