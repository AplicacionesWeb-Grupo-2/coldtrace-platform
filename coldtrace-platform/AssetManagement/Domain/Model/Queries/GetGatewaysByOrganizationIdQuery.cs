namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving gateways that belong to one organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetGatewaysByOrganizationIdQuery(int OrganizationId);
