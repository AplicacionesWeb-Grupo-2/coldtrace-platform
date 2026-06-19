namespace ColdTrace.Platform.AssetManagement.Domain.Model.Queries;

/// <summary>
///     Query for retrieving one IoT device by id and organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
/// <param name="IotDeviceId">IoT device identifier.</param>
public record GetIotDeviceByIdAndOrganizationIdQuery(int OrganizationId, int IotDeviceId);
