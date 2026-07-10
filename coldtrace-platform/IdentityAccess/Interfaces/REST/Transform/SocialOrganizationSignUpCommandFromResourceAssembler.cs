using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

public static class SocialOrganizationSignUpCommandFromResourceAssembler
{
    public static SocialOrganizationSignUpCommand ToCommandFromResource(
        string provider,
        SocialOrganizationSignUpResource resource) =>
        new(
            SocialProviderExtensions.FromCode(provider),
            resource.IdToken,
            resource.AuthorizationCode,
            resource.RedirectUri,
            resource.Nonce,
            resource.OrganizationName,
            resource.FullName);
}
