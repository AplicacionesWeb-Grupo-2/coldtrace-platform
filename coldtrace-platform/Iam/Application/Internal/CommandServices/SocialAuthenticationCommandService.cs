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
///     Application service for Google and Apple authentication.
/// </summary>
public class SocialAuthenticationCommandService(
    IExternalIdentityProviderService externalIdentityProviderService,
    IExternalIdentityRepository externalIdentityRepository,
    IUserRepository userRepository,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<SocialAuthenticationCommandService> logger)
    : ISocialAuthenticationCommandService
{
    /// <inheritdoc />
    public async Task<Result<AuthenticatedUserResult, SocialAuthenticationError>> Handle(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default)
    {
        var providerResult = await externalIdentityProviderService.ValidateAsync(command, cancellationToken);
        if (providerResult is Result<ProviderIdentity, SocialAuthenticationError>.Failure providerFailure)
            return new Result<AuthenticatedUserResult, SocialAuthenticationError>.Failure(providerFailure.Error);

        var providerIdentity = ((Result<ProviderIdentity, SocialAuthenticationError>.Success)providerResult).Value;

        try
        {
            var linkedIdentity = await externalIdentityRepository.FindByProviderAndSubjectAsync(
                providerIdentity.Provider,
                providerIdentity.Subject,
                cancellationToken);

            if (linkedIdentity is not null)
            {
                var linkedUser = await userRepository.FindByIdAsync(linkedIdentity.UserId, cancellationToken);
                if (linkedUser is null)
                {
                    logger.LogWarning(
                        "Social identity {ExternalIdentityId} points to missing user {UserId}",
                        linkedIdentity.Id,
                        linkedIdentity.UserId);
                    return Failure(SocialAuthenticationError.UserNotFound(linkedIdentity.UserId));
                }

                return Authenticated(linkedUser, providerIdentity.Provider);
            }

            var user = await FindAndLinkExistingUserAsync(providerIdentity, cancellationToken);
            if (user is null)
            {
                logger.LogInformation(
                    "Social identity requires onboarding for provider {Provider}; email present: {EmailPresent}",
                    providerIdentity.Provider.ToCode(),
                    !string.IsNullOrWhiteSpace(providerIdentity.Email));
                return Failure(SocialAuthenticationError.RequiresOnboarding());
            }

            return Authenticated(user, providerIdentity.Provider);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Social authentication failed for provider {Provider}", command.Provider.ToCode());
            return Failure(SocialAuthenticationError.Unexpected(
                "identity-access.authentication.error.social-authentication-failed"));
        }
    }

    private async Task<User?> FindAndLinkExistingUserAsync(
        ProviderIdentity providerIdentity,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(providerIdentity.Email)) return null;

        var user = await userRepository.FindByEmailAsync(providerIdentity.Email, cancellationToken);
        if (user is null) return null;

        try
        {
            await externalIdentityRepository.AddAsync(
                new ExternalIdentity(
                    providerIdentity.Provider,
                    providerIdentity.Subject,
                    providerIdentity.Email,
                    user.Id),
                cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation(
                "Linked social identity to user {UserId} for provider {Provider}",
                user.Id,
                providerIdentity.Provider.ToCode());
            return user;
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(
                ex,
                "Social identity link already exists after a concurrent request for provider {Provider}",
                providerIdentity.Provider.ToCode());
            var concurrentIdentity = await externalIdentityRepository.FindByProviderAndSubjectAsync(
                providerIdentity.Provider,
                providerIdentity.Subject,
                cancellationToken);
            return concurrentIdentity is null
                ? null
                : await userRepository.FindByIdAsync(concurrentIdentity.UserId, cancellationToken);
        }
    }

    private Result<AuthenticatedUserResult, SocialAuthenticationError> Authenticated(
        User user,
        SocialProvider provider)
    {
        var token = tokenService.GenerateToken(user);
        logger.LogInformation(
            "User {UserId} authenticated with provider {Provider}",
            user.Id,
            provider.ToCode());
        return new Result<AuthenticatedUserResult, SocialAuthenticationError>.Success(
            new AuthenticatedUserResult(user, token));
    }

    private static Result<AuthenticatedUserResult, SocialAuthenticationError> Failure(
        SocialAuthenticationError error) =>
        new Result<AuthenticatedUserResult, SocialAuthenticationError>.Failure(error);
}
