namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one gateway by id and organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="GatewayId">Gateway identifier.</param>
public record GetGatewayByIdAndOrganizationIdQuery(int OrganizationId, int GatewayId);
