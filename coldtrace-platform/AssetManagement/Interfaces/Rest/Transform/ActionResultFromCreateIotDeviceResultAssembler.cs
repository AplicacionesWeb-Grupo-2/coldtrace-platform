using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device creation result.
/// </summary>
public static class ActionResultFromCreateIotDeviceResultAssembler
{
    public static ActionResult ToActionResultFromCreateIotDeviceResult(
        Result<IotDevice, CreateIotDeviceError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
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
                        controller.ProblemResponse(localizer, "IotDeviceUuidDuplicated", StatusCodes.Status409Conflict),
                    CreateIotDeviceError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateIotDeviceError.GatewayNotFound =>
                        controller.ProblemResponse(localizer, "GatewayNotFound", StatusCodes.Status404NotFound),
                    CreateIotDeviceError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    CreateIotDeviceError.AssetLocationNotCompatible =>
                        controller.ProblemResponse(localizer, "IotDeviceAssetLocationNotCompatible", StatusCodes.Status409Conflict),
                    CreateIotDeviceError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingIotDevice", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
