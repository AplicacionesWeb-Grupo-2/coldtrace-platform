using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an organization resource from an organization aggregate.
/// </summary>
public static class OrganizationResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts an organization aggregate into a response resource.
    /// </summary>
    /// <param name="entity">The organization aggregate.</param>
    /// <returns>An organization response resource.</returns>
    public static OrganizationResource ToResourceFromEntity(Organization entity) =>
        new(entity.Id, entity.LegalName, entity.CommercialName, entity.TaxId, entity.ContactEmail);
}
