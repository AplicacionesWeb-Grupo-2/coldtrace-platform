using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a sensor reading lookup result.
/// </summary>
public static class ActionResultFromGetSensorReadingByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetSensorReadingByIdResult(
        Result<SensorReading, GetSensorReadingByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<MonitoringMessages> localizer) =>
        result switch
        {
            Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Success success =>
                controller.Ok(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetSensorReadingByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetSensorReadingByIdAndOrganizationError.SensorReadingNotFound =>
                        controller.ProblemResponse(localizer, "SensorReadingNotFound", StatusCodes.Status404NotFound),
                    GetSensorReadingByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingSensorReading", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
