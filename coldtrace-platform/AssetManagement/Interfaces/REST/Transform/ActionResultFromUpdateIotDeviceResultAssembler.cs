using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device update result.
/// </summary>
public static class ActionResultFromUpdateIotDeviceResultAssembler
{
    public static ActionResult ToActionResultFromUpdateIotDeviceResult(
        Result<IotDevice, UpdateIotDeviceError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IotDevice, UpdateIotDeviceError>.Success success =>
                controller.Ok(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<IotDevice, UpdateIotDeviceError>.Failure failure =>
                failure.Error switch
                {
                    UpdateIotDeviceError.DuplicateUuid =>
                        controller.Conflict(localizer["IotDeviceUuidDuplicated"].Value),
                    UpdateIotDeviceError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    UpdateIotDeviceError.GatewayNotFound =>
                        controller.NotFound(localizer["GatewayNotFound"].Value),
                    UpdateIotDeviceError.IotDeviceNotFound =>
                        controller.NotFound(localizer["IotDeviceNotFound"].Value),
                    UpdateIotDeviceError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    UpdateIotDeviceError.AssetLocationNotCompatible =>
                        controller.Conflict(localizer["IotDeviceAssetLocationNotCompatible"].Value),
                    UpdateIotDeviceError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorUpdatingIotDevice"].Value,
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
