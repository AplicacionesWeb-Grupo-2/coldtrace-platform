using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an organization sign-up resource from an application result.
/// </summary>
public static class OrganizationSignUpResourceFromResultAssembler
{
    /// <summary>
    ///     Converts an organization sign-up application result into a response resource.
    /// </summary>
    /// <param name="result">The organization sign-up application result.</param>
    /// <returns>An organization sign-up response resource.</returns>
    public static OrganizationSignUpResource ToResourceFromResult(OrganizationSignUpResult result) =>
        new(
            OrganizationResourceFromEntityAssembler.ToResourceFromEntity(result.Organization),
            UserResourceFromEntityAssembler.ToResourceFromEntity(result.User));
}
