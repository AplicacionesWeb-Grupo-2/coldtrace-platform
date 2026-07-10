using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.Internal.OutboundServices.Social;

/// <summary>
///     Validates identities issued by an external social provider.
/// </summary>
public interface IExternalIdentityProviderService
{
    Task<Result<ProviderIdentity, SocialAuthenticationError>> ValidateAsync(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default);
}
