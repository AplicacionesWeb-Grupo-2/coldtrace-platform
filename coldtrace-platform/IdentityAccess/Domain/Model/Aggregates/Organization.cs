using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;

/// <summary>
///     Organization aggregate root for the identity access context.
/// </summary>
public partial class Organization
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Organization()
    {
        LegalName = string.Empty;
        CommercialName = string.Empty;
        ContactEmail = string.Empty;
    }

    /// <summary>
    ///     Creates an organization from a creation command.
    /// </summary>
    /// <param name="command">Organization creation command.</param>
    public Organization(CreateOrganizationCommand command)
    {
        LegalName = command.LegalName;
        CommercialName = command.CommercialName;
        TaxId = command.TaxId;
        ContactEmail = command.ContactEmail;
    }

    /// <summary>
    ///     Gets the server-generated organization identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the organization legal name.
    /// </summary>
    public string LegalName { get; private set; }

    /// <summary>
    ///     Gets the organization commercial name.
    /// </summary>
    public string CommercialName { get; private set; }

    /// <summary>
    ///     Gets the optional organization tax identifier.
    /// </summary>
    public string? TaxId { get; private set; }

    /// <summary>
    ///     Gets the organization contact email.
    /// </summary>
    public string ContactEmail { get; private set; }
}
