using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Monitoring.Domain.Model.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.Rest.Transform;

/// <summary>
///     Converts demo sensor reading generation results to action results.
/// </summary>
public static class ActionResultFromGenerateDemoSensorReadingsResultAssembler
{
    public static ActionResult ToActionResultFromGenerateDemoSensorReadingsResult(
        Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError> result,
        ControllerBase controller,
        IStringLocalizer<MonitoringMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    success.Value.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Failure failure =>
                failure.Error switch
                {
                    GenerateDemoSensorReadingsError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GenerateDemoSensorReadingsError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    GenerateDemoSensorReadingsError.NoEligibleDevices =>
                        controller.ValidationProblemResponse(localizer, "InvalidRequest"),
                    GenerateDemoSensorReadingsError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingSensorReading", 500),
                    _ => controller.ValidationProblemResponse(localizer, "InvalidRequest")
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", StatusCodes.Status500InternalServerError, RestErrorCodes.UnexpectedError)
        };
}
