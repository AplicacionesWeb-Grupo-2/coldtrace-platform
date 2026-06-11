namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;

/// <summary>
///     Command for creating an organization.
/// </summary>
public record CreateOrganizationCommand
{
    /// <summary>
    ///     Creates a command with validated and normalized organization data.
    /// </summary>
    public CreateOrganizationCommand(string legalName, string commercialName, string? taxId, string contactEmail)
    {
        LegalName = RequireNonBlank(legalName);
        CommercialName = RequireNonBlank(commercialName);
        TaxId = NormalizeOptional(taxId);
        ContactEmail = RequireValidEmail(contactEmail);
    }

    /// <summary>
    ///     Gets the organization legal name.
    /// </summary>
    public string LegalName { get; init; }

    /// <summary>
    ///     Gets the organization commercial name.
    /// </summary>
    public string CommercialName { get; init; }

    /// <summary>
    ///     Gets the optional organization tax identifier.
    /// </summary>
    public string? TaxId { get; init; }

    /// <summary>
    ///     Gets the organization contact email.
    /// </summary>
    public string ContactEmail { get; init; }

    private static string RequireNonBlank(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Required value cannot be blank.");
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string RequireValidEmail(string? value)
    {
        var normalized = RequireNonBlank(value).ToLowerInvariant();
        if (!normalized.Contains('@')) throw new ArgumentException("Contact email is invalid.");
        return normalized;
    }
}
