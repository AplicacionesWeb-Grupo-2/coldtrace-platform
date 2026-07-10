using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a create organization command from a request resource.
/// </summary>
public static class CreateOrganizationCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a create organization resource into a command.
    /// </summary>
    /// <param name="resource">The create organization resource.</param>
    /// <returns>A create organization command.</returns>
    public static CreateOrganizationCommand ToCommandFromResource(CreateOrganizationResource resource) =>
        new(resource.LegalName, resource.CommercialName, resource.TaxId, resource.ContactEmail);
}
