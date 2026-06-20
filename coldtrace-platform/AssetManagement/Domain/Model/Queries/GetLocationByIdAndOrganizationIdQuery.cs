namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one location by id and organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="LocationId">Location identifier.</param>
public record GetLocationByIdAndOrganizationIdQuery(int OrganizationId, int LocationId);
