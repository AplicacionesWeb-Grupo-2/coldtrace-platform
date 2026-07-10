using System.Buffers.Text;
using System.Security.Cryptography;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Internal.OutboundServices;
using ColdTrace.Platform.Iam.Application.Internal.OutboundServices.Social;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Iam.Domain.Repositories;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Iam.Application.Internal.CommandServices;

/// <summary>
///     Creates an organization and first user from a verified social identity.
/// </summary>
public class SocialOrganizationSignUpCommandService(
    IExternalIdentityProviderService externalIdentityProviderService,
    IExternalIdentityRepository externalIdentityRepository,
    IOrganizationRepository organizationRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<SocialOrganizationSignUpCommandService> logger)
    : ISocialOrganizationSignUpCommandService
{
    private const string InitialRoleName = "super-admin";

    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserResult, SocialAuthenticationError>> Handle(
        SocialOrganizationSignUpCommand command,
        CancellationToken cancellationToken = default)
    {
        var providerResult = await externalIdentityProviderService.ValidateAsync(
            command.ToSocialSignInCommand(),
            cancellationToken);
        if (providerResult is Result<ProviderIdentity, SocialAuthenticationError>.Failure providerFailure)
            return Failure(providerFailure.Error);

        var identity = ((Result<ProviderIdentity, SocialAuthenticationError>.Success)providerResult).Value;
        var linkedIdentity = await externalIdentityRepository.FindByProviderAndSubjectAsync(
            identity.Provider,
            identity.Subject,
            cancellationToken);
        if (linkedIdentity is not null)
        {
            var linkedUser = await userRepository.FindByIdAsync(linkedIdentity.UserId, cancellationToken);
            if (linkedUser is not null)
            {
                logger.LogWarning(
                    "Social organization sign-up attempted with linked user {UserId} for provider {Provider}",
                    linkedUser.Id,
                    identity.Provider.ToCode());
                return Failure(SocialAuthenticationError.UserConflict());
            }
        }

        var providerEmail = identity.Email;
        if (string.IsNullOrWhiteSpace(providerEmail))
            return Failure(SocialAuthenticationError.ProviderValidationFailed());

        if (await userRepository.ExistsByEmailAsync(providerEmail, cancellationToken))
            return Failure(SocialAuthenticationError.UserConflict());

        if (await organizationRepository.ExistsByContactEmailAsync(providerEmail, cancellationToken))
            return Failure(SocialAuthenticationError.OrganizationConflict());

        var role = await roleRepository.FindByNameAsync(InitialRoleName, cancellationToken);
        if (role is null)
            return Failure(SocialAuthenticationError.Unexpected(
                "identity-access.organization-sign-up.error.initial-role-not-found"));

        try
        {
            return await unitOfWork.ExecuteInTransactionAsync(async transactionCancellationToken =>
            {
                var organization = new Organization(new CreateOrganizationCommand(
                    command.OrganizationName,
                    command.OrganizationName,
                    null,
                    providerEmail));
                var name = SplitName(command.FullName);
                var providerManagedPassword = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(32));
                var userCommand = new CreateUserCommand(
                    name.FirstName,
                    name.LastName,
                    providerEmail,
                    providerManagedPassword);
                var user = new User(
                    userCommand,
                    organization,
                    role,
                    hashingService.HashPassword(providerManagedPassword));

                await organizationRepository.AddAsync(organization, transactionCancellationToken);
                await userRepository.AddAsync(user, transactionCancellationToken);
                await unitOfWork.CompleteAsync(transactionCancellationToken);

                await subscriptionBillingContextFacade.InitializeBaseSubscriptionForOrganizationAsync(
                    organization.Id,
                    transactionCancellationToken);

                await externalIdentityRepository.AddAsync(
                    new ExternalIdentity(identity.Provider, identity.Subject, providerEmail, user.Id),
                    transactionCancellationToken);
                await unitOfWork.CompleteAsync(transactionCancellationToken);

                logger.LogInformation(
                    "Social organization sign-up completed for organization {OrganizationId}, user {UserId}, provider {Provider}",
                    organization.Id,
                    user.Id,
                    identity.Provider.ToCode());
                return new Result<AuthenticatedUserResult, SocialAuthenticationError>.Success(
                    new AuthenticatedUserResult(user, tokenService.GenerateToken(user)));
            }, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(
                ex,
                "Social organization sign-up conflict for provider {Provider}",
                identity.Provider.ToCode());
            return Failure(SocialAuthenticationError.SocialIdentityConflict());
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Social organization sign-up failed for provider {Provider}",
                identity.Provider.ToCode());
            return Failure(SocialAuthenticationError.Unexpected(
                "identity-access.authentication.error.social-organization-sign-up-failed"));
        }
    }

    private static NameParts SplitName(string fullName)
    {
        var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new NameParts(names[0], string.Join(' ', names.Skip(1)));
    }

    private static Result<AuthenticatedUserResult, SocialAuthenticationError> Failure(
        SocialAuthenticationError error) =>
        new Result<AuthenticatedUserResult, SocialAuthenticationError>.Failure(error);

    private sealed record NameParts(string FirstName, string LastName);
}
