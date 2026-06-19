using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device lookup result.
/// </summary>
public static class ActionResultFromGetIotDeviceByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetIotDeviceByIdResult(
        Result<IotDevice, GetIotDeviceByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Success success =>
                controller.Ok(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIotDeviceByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetIotDeviceByIdAndOrganizationError.IotDeviceNotFound =>
                        controller.NotFound(localizer["IotDeviceNotFound"].Value),
                    GetIotDeviceByIdAndOrganizationError.UnexpectedError =>
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
