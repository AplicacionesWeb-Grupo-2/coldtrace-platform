using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Monitoring.Interfaces.REST.Transform;

/// <summary>
///     Converts demo sensor reading generation results to action results.
/// </summary>
public static class ActionResultFromGenerateDemoSensorReadingsResultAssembler
{
    public static ActionResult ToActionResultFromGenerateDemoSensorReadingsResult(
        Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
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
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GenerateDemoSensorReadingsError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    GenerateDemoSensorReadingsError.NoEligibleDevices =>
                        controller.BadRequest(localizer["InvalidRequest"].Value),
                    GenerateDemoSensorReadingsError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingSensorReading"].Value,
                            statusCode: 500),
                    _ => controller.BadRequest(localizer["InvalidRequest"].Value)
                },
            _ => controller.Problem()
        };
}
