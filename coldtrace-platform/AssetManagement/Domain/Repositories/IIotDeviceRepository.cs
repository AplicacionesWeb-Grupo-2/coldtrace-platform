using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.AssetManagement.Domain.Repositories;

/// <summary>
///     IoT device repository contract.
/// </summary>
public interface IIotDeviceRepository : IBaseRepository<IotDevice>
{
    /// <summary>
    ///     Finds all IoT devices that belong to an organization.
    /// </summary>
    Task<IEnumerable<IotDevice>> FindAllByOrganizationIdAsync(
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds one IoT device by id and organization.
    /// </summary>
    Task<IotDevice?> FindByIdAndOrganizationIdAsync(
        int id,
        int organizationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether an IoT device UUID already exists in an organization.
    /// </summary>
    Task<bool> ExistsByOrganizationIdAndUuidAsync(
        int organizationId,
        string uuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether another IoT device already uses a UUID in an organization.
    /// </summary>
    Task<bool> ExistsByOrganizationIdAndUuidAndIdNotAsync(
        int organizationId,
        string uuid,
        int id,
        CancellationToken cancellationToken = default);
}
