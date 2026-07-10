using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.Internal.QueryServices;

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
    public Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default) =>
        userRepository.FindByIdAsync(query.UserId, cancellationToken);

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
