using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;

/// <summary>
///     Application service for organization sign-up operations.
/// </summary>
/// <param name="organizationRepository">Organization repository.</param>
/// <param name="userRepository">User repository.</param>
/// <param name="roleRepository">Role repository.</param>
/// <param name="unitOfWork">Unit of work for persistence.</param>
/// <param name="logger">Logger for diagnostics.</param>
public class OrganizationSignUpCommandService(
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    ILogger<OrganizationSignUpCommandService> logger)
    : IOrganizationSignUpCommandService
{
    private const string InitialRoleName = "super-admin";

    /// <inheritdoc />
    public async Task<Result<OrganizationSignUpResult, CreateOrganizationSignUpError>> Handle(
        CreateOrganizationSignUpCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await organizationRepository.ExistsByContactEmailAsync(command.ContactEmail, cancellationToken))
        {
            logger.LogWarning("Duplicate organization contact email rejected during sign-up: {ContactEmail}",
                command.ContactEmail);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.DuplicateOrganizationContactEmail);
        }

        if (!string.IsNullOrWhiteSpace(command.TaxId) &&
            await organizationRepository.ExistsByTaxIdAsync(command.TaxId, cancellationToken))
        {
            logger.LogWarning("Duplicate organization tax id rejected during sign-up: {TaxId}", command.TaxId);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.DuplicateOrganizationTaxId);
        }

        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            logger.LogWarning("Duplicate user email rejected during sign-up: {Email}", command.Email);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.DuplicateUserEmail);
        }

        var initialRole = await roleRepository.FindByNameAsync(InitialRoleName, cancellationToken);
        if (initialRole is null)
        {
            logger.LogError("Initial sign-up role not found: {RoleName}", InitialRoleName);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.InitialRoleNotFound);
        }

        try
        {
            var organization = new Organization(command.ToCreateOrganizationCommand());
            var user = new User(command.ToCreateUserCommand(), organization, initialRole);

            await organizationRepository.AddAsync(organization, cancellationToken);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Success(
                new OrganizationSignUpResult(organization, user));
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateKeyError(ex, out var duplicateError))
            {
                logger.LogWarning(ex, "Duplicate key violation during organization sign-up for {Email}",
                    command.Email);
                return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(duplicateError);
            }

            logger.LogError(ex, "Database update failed during organization sign-up for {Email}", command.Email);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during organization sign-up for {Email}", command.Email);
            return new Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure(
                CreateOrganizationSignUpError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateKeyError(
        DbUpdateException exception,
        out CreateOrganizationSignUpError duplicateError)
    {
        duplicateError = CreateOrganizationSignUpError.DuplicateOrganizationContactEmail;

        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062)
            {
                if (current.Message.Contains("i_x_organizations_tax_id", StringComparison.OrdinalIgnoreCase))
                    duplicateError = CreateOrganizationSignUpError.DuplicateOrganizationTaxId;
                if (current.Message.Contains("i_x_users_email", StringComparison.OrdinalIgnoreCase))
                    duplicateError = CreateOrganizationSignUpError.DuplicateUserEmail;

                return true;
            }
        }

        return false;
    }
}
