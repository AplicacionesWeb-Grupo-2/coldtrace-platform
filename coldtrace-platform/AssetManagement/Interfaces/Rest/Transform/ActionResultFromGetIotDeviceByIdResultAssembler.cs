using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT device lookup result.
/// </summary>
public static class ActionResultFromGetIotDeviceByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetIotDeviceByIdResult(
        Result<IotDevice, GetIotDeviceByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Success success =>
                controller.Ok(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<IotDevice, GetIotDeviceByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIotDeviceByIdAndOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetIotDeviceByIdAndOrganizationError.IotDeviceNotFound =>
                        controller.ProblemResponse(localizer, "IotDeviceNotFound", StatusCodes.Status404NotFound),
                    GetIotDeviceByIdAndOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingIotDevices", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
