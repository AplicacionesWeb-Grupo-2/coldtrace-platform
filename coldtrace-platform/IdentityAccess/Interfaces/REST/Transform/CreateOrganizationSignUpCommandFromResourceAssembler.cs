using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an organization sign-up command from a request resource.
/// </summary>
public static class CreateOrganizationSignUpCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create organization sign-up resource into a command.
    /// </summary>
    /// <param name="resource">The create organization sign-up resource.</param>
    /// <returns>A create organization sign-up command.</returns>
    public static CreateOrganizationSignUpCommand ToCommandFromResource(CreateOrganizationSignUpResource resource) =>
        new(
            resource.LegalName,
            resource.CommercialName,
            resource.TaxId,
            resource.ContactEmail,
            resource.FirstName,
            resource.LastName,
            resource.Email,
            resource.Password);
}
