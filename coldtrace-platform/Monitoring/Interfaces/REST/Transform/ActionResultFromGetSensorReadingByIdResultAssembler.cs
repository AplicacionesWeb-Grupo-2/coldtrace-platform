using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a sensor reading lookup result.
/// </summary>
public static class ActionResultFromGetSensorReadingByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetSensorReadingByIdResult(
        Result<SensorReading, GetSensorReadingByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Success success =>
                controller.Ok(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<SensorReading, GetSensorReadingByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetSensorReadingByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetSensorReadingByIdAndOrganizationError.SensorReadingNotFound =>
                        controller.NotFound(localizer["SensorReadingNotFound"].Value),
                    GetSensorReadingByIdAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingSensorReading"].Value,
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
