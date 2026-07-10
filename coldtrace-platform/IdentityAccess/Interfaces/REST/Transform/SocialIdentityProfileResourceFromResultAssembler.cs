using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

public static class SocialIdentityProfileResourceFromResultAssembler
{
    public static SocialIdentityProfileResource ToResourceFromResult(SocialIdentityProfileResult result) =>
        new(result.IdToken, result.Email, result.FullName);
}
