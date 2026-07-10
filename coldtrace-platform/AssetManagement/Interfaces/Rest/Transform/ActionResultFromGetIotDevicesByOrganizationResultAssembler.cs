using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an IoT devices listing result.
/// </summary>
public static class ActionResultFromGetIotDevicesByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetIotDevicesByOrganizationResult(
        Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(IotDeviceResourceFromEntityAssembler.ToResourceFromEntity)),
            Result<IEnumerable<IotDevice>, GetIotDevicesByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetIotDevicesByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetIotDevicesByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingIotDevices", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },
            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
