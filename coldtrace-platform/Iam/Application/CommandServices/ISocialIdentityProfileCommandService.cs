using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.CommandServices;

public interface ISocialIdentityProfileCommandService
{
    Task<Result<SocialIdentityProfileResult, SocialAuthenticationError>> Handle(
        SocialSignInCommand command,
        CancellationToken cancellationToken = default);
}
