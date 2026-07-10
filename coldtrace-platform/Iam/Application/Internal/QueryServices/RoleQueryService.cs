using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Iam.Domain.Repositories;

namespace ColdTrace.Platform.Iam.Application.Internal.QueryServices;

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
