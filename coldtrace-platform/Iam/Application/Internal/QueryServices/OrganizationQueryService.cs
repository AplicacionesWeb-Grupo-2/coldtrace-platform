using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Iam.Domain.Repositories;

namespace ColdTrace.Platform.Iam.Application.Internal.QueryServices;

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
