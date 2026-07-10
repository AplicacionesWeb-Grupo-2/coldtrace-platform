using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization sensor readings listing result.
/// </summary>
public static class ActionResultFromGetSensorReadingsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetSensorReadingsByOrganizationResult(
        Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<MonitoringMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<SensorReading>, GetSensorReadingsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetSensorReadingsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetSensorReadingsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingSensorReadings", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
