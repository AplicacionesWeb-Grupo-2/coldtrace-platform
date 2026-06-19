using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a sensor reading creation result.
/// </summary>
public static class ActionResultFromCreateSensorReadingResultAssembler
{
    public static ActionResult ToActionResultFromCreateSensorReadingResult(
        Result<SensorReading, CreateSensorReadingError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
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
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    CreateSensorReadingError.IotDeviceNotFound =>
                        controller.NotFound(localizer["IotDeviceNotFound"].Value),
                    CreateSensorReadingError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingSensorReading"].Value,
                            statusCode: 500),
                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
