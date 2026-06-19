namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving all asset settings that belong to one organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetAssetSettingsByOrganizationIdQuery(int OrganizationId);