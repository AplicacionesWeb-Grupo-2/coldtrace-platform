using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

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
