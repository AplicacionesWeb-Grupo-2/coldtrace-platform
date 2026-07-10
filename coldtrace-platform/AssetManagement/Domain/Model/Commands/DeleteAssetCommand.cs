namespace ColdTrace.Platform.AssetManagement.Domain.Model.Commands;

/// <summary>
///     Command for deleting an asset inside an organization.
/// </summary>
public record DeleteAssetCommand
{
    /// <summary>
    ///     Creates a command with validated organization-scoped identifiers.
    /// </summary>
    public DeleteAssetCommand(int organizationId, int assetId)
    {
        if (organizationId <= 0) throw new ArgumentException("AssetOrganizationIdInvalid");
        if (assetId <= 0) throw new ArgumentException("AssetIdInvalid");

        OrganizationId = organizationId;
        AssetId = assetId;
    }

    /// <summary>Gets the organization identifier that scopes the asset.</summary>
    public int OrganizationId { get; init; }

    /// <summary>Gets the asset identifier to delete.</summary>
    public int AssetId { get; init; }
}
