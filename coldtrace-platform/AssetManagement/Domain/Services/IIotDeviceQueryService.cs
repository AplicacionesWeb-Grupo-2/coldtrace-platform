using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Query service contract for IoT devices.
/// </summary>
public interface IIotDeviceQueryService
{
    /// <summary>
    ///     Handles the get devices by organization use case.
    /// </summary>
    Task<Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>> Handle(
        GetIotDevicesByOrganizationIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles the get device by id use case.
    /// </summary>
    Task<Result<IotDevice, GetIotDeviceByIdAndOrganizationError>> Handle(
        GetIotDeviceByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default);
}
