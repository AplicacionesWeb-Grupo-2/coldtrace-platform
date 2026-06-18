using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.QueryServices;

/// <summary>
///     Application service for organization query operations.
/// </summary>
/// <param name="organizationRepository">Organization repository.</param>
public class OrganizationQueryService(IOrganizationRepository organizationRepository)
    : IOrganizationQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Organization>> Handle(
        GetAllOrganizationsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await organizationRepository.ListAsync(cancellationToken);
    }
}
