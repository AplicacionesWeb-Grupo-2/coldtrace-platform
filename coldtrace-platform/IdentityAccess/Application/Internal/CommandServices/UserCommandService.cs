using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;

/// <summary>
///     Application service for user command operations.
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserCommandService> logger)
    : IUserCommandService
{
    /// <inheritdoc />
    public async Task<Result<User, CreateUserError>> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            logger.LogWarning("Duplicate user email rejected: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.DuplicateEmail);
        }

        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for user creation: {OrganizationId}", command.OrganizationId);
            return new Result<User, CreateUserError>.Failure(CreateUserError.OrganizationNotFound);
        }

        var role = await roleRepository.FindByIdAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            logger.LogWarning("Role not found for user creation: {RoleId}", command.RoleId);
            return new Result<User, CreateUserError>.Failure(CreateUserError.RoleNotFound);
        }

        try
        {
            var user = new User(command);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, CreateUserError>.Success(user);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateEmailError(ex))
            {
                logger.LogWarning(ex, "Duplicate key violation creating user with email {Email}", command.Email);
                return new Result<User, CreateUserError>.Failure(CreateUserError.DuplicateEmail);
            }

            logger.LogError(ex, "Database update failed creating user with email {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating user with email {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateEmailError(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062 &&
                current.Message.Contains("i_x_users_email", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
