using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a device sensor readings listing result.
/// </summary>
public static class ActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResult(
        Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetSensorReadingsByIotDeviceAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetSensorReadingsByIotDeviceAndOrganizationError.IotDeviceNotFound =>
                        controller.NotFound(localizer["IotDeviceNotFound"].Value),
                    GetSensorReadingsByIotDeviceAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingSensorReadings"].Value,
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
