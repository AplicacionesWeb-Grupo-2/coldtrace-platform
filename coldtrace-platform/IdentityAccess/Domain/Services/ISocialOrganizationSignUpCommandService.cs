using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Domain.Services;

public interface ISocialOrganizationSignUpCommandService
{
    Task<Result<AuthenticatedUserResult, SocialAuthenticationError>> Handle(
        SocialOrganizationSignUpCommand command,
        CancellationToken cancellationToken = default);
}
