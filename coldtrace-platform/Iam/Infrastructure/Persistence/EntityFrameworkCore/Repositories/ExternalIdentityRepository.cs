using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework repository for external identity links.
/// </summary>
public class ExternalIdentityRepository(AppDbContext context)
    : BaseRepository<ExternalIdentity>(context), IExternalIdentityRepository
{
    /// <inheritdoc />
    public async Task<ExternalIdentity?> FindByProviderAndSubjectAsync(
        SocialProvider provider,
        string providerSubject,
        CancellationToken cancellationToken = default)
    {
        var normalizedSubject = providerSubject.Trim();
        return await Context.Set<ExternalIdentity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                identity => identity.Provider == provider && identity.ProviderSubject == normalizedSubject,
                cancellationToken);
    }
}
