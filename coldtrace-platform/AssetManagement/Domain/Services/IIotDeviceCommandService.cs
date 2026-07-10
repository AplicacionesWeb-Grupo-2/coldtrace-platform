using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Command service contract for IoT devices.
/// </summary>
public interface IIotDeviceCommandService
{
    /// <summary>
    ///     Handles the create IoT device use case.
    /// </summary>
    Task<Result<IotDevice, CreateIotDeviceError>> Handle(
        CreateIotDeviceCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles the update IoT device use case.
    /// </summary>
    Task<Result<IotDevice, UpdateIotDeviceError>> Handle(
        UpdateIotDeviceCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles the delete IoT device use case.
    /// </summary>
    Task<Result<DeleteIotDeviceCommand, DeleteIotDeviceError>> Handle(
        DeleteIotDeviceCommand command,
        CancellationToken cancellationToken = default);
}
