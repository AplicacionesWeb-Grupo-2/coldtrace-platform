using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

public static class SocialIdentityProfileResourceFromResultAssembler
{
    public static SocialIdentityProfileResource ToResourceFromResult(SocialIdentityProfileResult result) =>
        new(result.IdToken, result.Email, result.FullName);
}
