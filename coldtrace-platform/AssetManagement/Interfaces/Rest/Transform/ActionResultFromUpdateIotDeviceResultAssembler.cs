using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device update result.
/// </summary>
public static class ActionResultFromUpdateIotDeviceResultAssembler
{
    public static ActionResult ToActionResultFromUpdateIotDeviceResult(
        Result<IotDevice, UpdateIotDeviceError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<IotDevice, UpdateIotDeviceError>.Success success =>
                controller.Ok(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<IotDevice, UpdateIotDeviceError>.Failure failure =>
                failure.Error switch
                {
                    UpdateIotDeviceError.DuplicateUuid =>
                        controller.ProblemResponse(localizer, "IotDeviceUuidDuplicated", StatusCodes.Status409Conflict),
                    UpdateIotDeviceError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    UpdateIotDeviceError.GatewayNotFound =>
                        controller.ProblemResponse(localizer, "GatewayNotFound", StatusCodes.Status404NotFound),
                    UpdateIotDeviceError.IotDeviceNotFound =>
                        controller.ProblemResponse(localizer, "IotDeviceNotFound", StatusCodes.Status404NotFound),
                    UpdateIotDeviceError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    UpdateIotDeviceError.AssetLocationNotCompatible =>
                        controller.ProblemResponse(localizer, "IotDeviceAssetLocationNotCompatible", StatusCodes.Status409Conflict),
                    UpdateIotDeviceError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorUpdatingIotDevice", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
