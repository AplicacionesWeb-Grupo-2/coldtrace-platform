using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices.Social;

/// <summary>
///     Validates identities issued by an external social provider.
/// </summary>
public interface IExternalIdentityProviderService
{
    Task<Result<ProviderIdentity, SocialAuthenticationError>> ValidateAsync(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default);
}
