using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

public static class SocialSignInCommandFromResourceAssembler
{
    public static SocialSignInCommand ToCommandFromResource(
        string provider,
        SocialTokenExchangeResource resource) =>
        new(
            SocialProviderExtensions.FromCode(provider),
            resource.IdToken,
            resource.AuthorizationCode,
            resource.RedirectUri,
            resource.Nonce);
}
