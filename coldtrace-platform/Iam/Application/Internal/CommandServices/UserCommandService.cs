using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Internal.OutboundServices;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Iam.Application.Internal.CommandServices;

/// <summary>
///     Application service for user command operations.
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    IOrganizationRepository organizationRepository,
    IRoleRepository roleRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<UserCommandService> logger)
    : IUserCommandService
{
    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserResult, AuthenticationError>> Handle(
        SignInCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);
        if (user is null ||
            string.IsNullOrWhiteSpace(user.PasswordHash) ||
            !hashingService.VerifyPassword(command.Password, user.PasswordHash))
        {
            logger.LogWarning("Invalid sign-in credentials for email {Email}", command.Email);
            return new Result<AuthenticatedUserResult, AuthenticationError>.Failure(
                AuthenticationError.InvalidCredentials);
        }

        var token = tokenService.GenerateToken(user);
        return new Result<AuthenticatedUserResult, AuthenticationError>.Success(
            new AuthenticatedUserResult(user, token));
    }

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

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementUsers,
            "UserPlanLimitExceeded",
            cancellationToken);

        try
        {
            var passwordHash = hashingService.HashPassword(command.Password);
            var user = new User(command, passwordHash);
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

    /// <inheritdoc />
    public async Task<Result<User, AssignUserRoleError>> Handle(
        AssignUserRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning(
                "Organization not found for user role assignment: {OrganizationId}",
                command.OrganizationId);
            return new Result<User, AssignUserRoleError>.Failure(AssignUserRoleError.OrganizationNotFound);
        }

        var user = await userRepository.FindByIdAndOrganizationIdAsync(
            command.UserId,
            command.OrganizationId,
            cancellationToken);
        if (user is null)
        {
            logger.LogWarning(
                "User not found for role assignment: {OrganizationId} {UserId}",
                command.OrganizationId,
                command.UserId);
            return new Result<User, AssignUserRoleError>.Failure(AssignUserRoleError.UserNotFound);
        }

        var role = await roleRepository.FindByIdAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            logger.LogWarning("Role not found for user role assignment: {RoleId}", command.RoleId);
            return new Result<User, AssignUserRoleError>.Failure(AssignUserRoleError.RoleNotFound);
        }

        try
        {
            user.AssignRole(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, AssignUserRoleError>.Success(user);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Database update failed assigning role {RoleId} to user {UserId} in organization {OrganizationId}",
                command.RoleId,
                command.UserId,
                command.OrganizationId);
            return new Result<User, AssignUserRoleError>.Failure(AssignUserRoleError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error assigning role {RoleId} to user {UserId} in organization {OrganizationId}",
                command.RoleId,
                command.UserId,
                command.OrganizationId);
            return new Result<User, AssignUserRoleError>.Failure(AssignUserRoleError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<DeleteUserCommand, DeleteUserError>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for user deletion: {OrganizationId}", command.OrganizationId);
            return new Result<DeleteUserCommand, DeleteUserError>.Failure(DeleteUserError.OrganizationNotFound);
        }

        var user = await userRepository.FindByIdAndOrganizationIdAsync(
            command.UserId,
            command.OrganizationId,
            cancellationToken);
        if (user is null)
        {
            logger.LogWarning(
                "User not found for deletion: {OrganizationId} {UserId}",
                command.OrganizationId,
                command.UserId);
            return new Result<DeleteUserCommand, DeleteUserError>.Failure(DeleteUserError.UserNotFound);
        }

        try
        {
            userRepository.Remove(user);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<DeleteUserCommand, DeleteUserError>.Success(command);
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(
                ex,
                "User deletion blocked by related records: {OrganizationId} {UserId}",
                command.OrganizationId,
                command.UserId);
            return new Result<DeleteUserCommand, DeleteUserError>.Failure(DeleteUserError.DeleteBlocked);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error deleting user {UserId} from organization {OrganizationId}",
                command.UserId,
                command.OrganizationId);
            return new Result<DeleteUserCommand, DeleteUserError>.Failure(DeleteUserError.UnexpectedError);
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
