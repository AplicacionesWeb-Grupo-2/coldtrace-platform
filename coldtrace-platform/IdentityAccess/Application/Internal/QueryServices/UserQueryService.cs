using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.QueryServices;

/// <summary>
///     Application service for user query operations.
/// </summary>
public class UserQueryService(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    ILogger<UserQueryService> logger)
    : IUserQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<User>, GetUsersByOrganizationError>> Handle(
        GetUsersByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for user query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<User>, GetUsersByOrganizationError>.Failure(
                GetUsersByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var users = await userRepository.FindAllByOrganizationIdAsync(query.OrganizationId, cancellationToken);
            return new Result<IEnumerable<User>, GetUsersByOrganizationError>.Success(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error querying users for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<User>, GetUsersByOrganizationError>.Failure(
                GetUsersByOrganizationError.UnexpectedError);
        }
    }
}
