using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT devices listing result.
/// </summary>
public static class ActionResultFromGetIotDevicesByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetIotDevicesByOrganizationResult(
        Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIotDevicesByOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetIotDevicesByOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingIotDevices"].Value,
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
