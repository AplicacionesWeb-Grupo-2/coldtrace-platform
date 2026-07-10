using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

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
