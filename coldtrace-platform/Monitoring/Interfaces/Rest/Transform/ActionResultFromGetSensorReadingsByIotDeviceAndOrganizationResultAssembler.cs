using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a device sensor readings listing result.
/// </summary>
public static class ActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetSensorReadingsByIotDeviceAndOrganizationResult(
        Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<MonitoringMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<SensorReading>, GetSensorReadingsByIotDeviceAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetSensorReadingsByIotDeviceAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetSensorReadingsByIotDeviceAndOrganizationError.IotDeviceNotFound =>
                        controller.ProblemResponse(localizer, "IotDeviceNotFound", StatusCodes.Status404NotFound),
                    GetSensorReadingsByIotDeviceAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingSensorReadings", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
