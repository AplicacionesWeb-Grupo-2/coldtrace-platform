using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Entity Framework repository for organization persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class OrganizationRepository(AppDbContext context)
    : BaseRepository<Organization>(context), IOrganizationRepository
{
    /// <inheritdoc />
    public async Task<bool> ExistsByContactEmailAsync(
        string contactEmail,
        CancellationToken cancellationToken = default)
    {
        var normalizedContactEmail = contactEmail.Trim().ToLowerInvariant();
        return await Context.Set<Organization>()
            .AnyAsync(organization => organization.ContactEmail == normalizedContactEmail, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        var normalizedTaxId = taxId.Trim().ToLowerInvariant();
        return await Context.Set<Organization>()
            .AnyAsync(organization =>
                    organization.TaxId != null && organization.TaxId.ToLower() == normalizedTaxId,
                cancellationToken);
    }
}
