using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a sensor reading creation result.
/// </summary>
public static class ActionResultFromCreateSensorReadingResultAssembler
{
    public static ActionResult ToActionResultFromCreateSensorReadingResult(
        Result<SensorReading, CreateSensorReadingError> result,
        ControllerBase controller,
        IStringLocalizer<MonitoringMessages> localizer) =>
        result switch
        {
            Result<SensorReading, CreateSensorReadingError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<SensorReading, CreateSensorReadingError>.Failure failure =>
                failure.Error switch
                {
                    CreateSensorReadingError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateSensorReadingError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    CreateSensorReadingError.IotDeviceNotFound =>
                        controller.ProblemResponse(localizer, "IotDeviceNotFound", StatusCodes.Status404NotFound),
                    CreateSensorReadingError.GatewayNotFound =>
                        controller.ProblemResponse(localizer, "GatewayNotFound", StatusCodes.Status404NotFound),
                    CreateSensorReadingError.DeviceNotAssignedToAsset or
                        CreateSensorReadingError.IncompatibleLocation or
                        CreateSensorReadingError.DeviceOffline or
                        CreateSensorReadingError.GatewayOffline or
                        CreateSensorReadingError.AssetSettingsNotFound or
                        CreateSensorReadingError.UnsupportedMeasurement =>
                        controller.ValidationProblemResponse(localizer, "InvalidSensorReadingRequest"),
                    CreateSensorReadingError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingSensorReading", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
