using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Iam.Interfaces.Acl;

namespace ColdTrace.Platform.Iam.Application.Acl;

/// <summary>
///     IAM anti-corruption layer used by the remaining bounded contexts.
/// </summary>
public class IamContextFacade(IOrganizationRepository organizationRepository) : IIamContextFacade
{
    /// <inheritdoc />
    public async Task<bool> OrganizationExistsAsync(
        int organizationId,
        CancellationToken cancellationToken = default) =>
        await organizationRepository.FindByIdAsync(organizationId, cancellationToken) is not null;

    /// <inheritdoc />
    public async Task<IEnumerable<int>> ListOrganizationIdsAsync(CancellationToken cancellationToken = default) =>
        (await organizationRepository.ListAsync(cancellationToken)).Select(organization => organization.Id);
}
