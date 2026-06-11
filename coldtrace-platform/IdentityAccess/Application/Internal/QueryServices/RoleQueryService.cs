using ColdTrace.Platform.IdentityAccess.Application.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.QueryServices;

/// <summary>
///     Application service for role query operations.
/// </summary>
public class RoleQueryService(IRoleRepository roleRepository) : IRoleQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Role>> Handle(
        GetAllRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        return await roleRepository.ListWithPermissionsAsync(cancellationToken);
    }
}
