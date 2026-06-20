using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device creation result.
/// </summary>
public static class ActionResultFromCreateIotDeviceResultAssembler
{
    public static ActionResult ToActionResultFromCreateIotDeviceResult(
        Result<IotDevice, CreateIotDeviceError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IotDevice, CreateIotDeviceError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    IotDeviceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<IotDevice, CreateIotDeviceError>.Failure failure =>
                failure.Error switch
                {
                    CreateIotDeviceError.DuplicateUuid =>
                        controller.Conflict(localizer["IotDeviceUuidDuplicated"].Value),
                    CreateIotDeviceError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    CreateIotDeviceError.GatewayNotFound =>
                        controller.NotFound(localizer["GatewayNotFound"].Value),
                    CreateIotDeviceError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    CreateIotDeviceError.AssetLocationNotCompatible =>
                        controller.Conflict(localizer["IotDeviceAssetLocationNotCompatible"].Value),
                    CreateIotDeviceError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingIotDevice"].Value,
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
