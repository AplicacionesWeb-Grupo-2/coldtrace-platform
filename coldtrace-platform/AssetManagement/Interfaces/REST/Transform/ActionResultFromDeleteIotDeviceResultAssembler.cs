using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Converts IoT device deletion results into HTTP responses.
/// </summary>
public static class ActionResultFromDeleteIotDeviceResultAssembler
{
    /// <summary>
    ///     Returns 204 on success or a localized ProblemDetails response on failure.
    /// </summary>
    public static ActionResult ToActionResultFromDeleteIotDeviceResult(
        Result<DeleteIotDeviceCommand, DeleteIotDeviceError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer)
    {
        return result switch
        {
            Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Success => controller.NoContent(),
            Result<DeleteIotDeviceCommand, DeleteIotDeviceError>.Failure failure => failure.Error switch
            {
                DeleteIotDeviceError.OrganizationNotFound => controller.Problem(
                    detail: localizer["OrganizationNotFound"].Value,
                    statusCode: StatusCodes.Status404NotFound),
                DeleteIotDeviceError.IotDeviceNotFound => controller.Problem(
                    detail: localizer["IotDeviceNotFound"].Value,
                    statusCode: StatusCodes.Status404NotFound),
                DeleteIotDeviceError.DeleteBlocked => controller.Problem(
                    detail: localizer["IotDeviceDeleteBlocked"].Value,
                    statusCode: StatusCodes.Status409Conflict),
                _ => controller.Problem(
                    title: localizer["UnexpectedServerError"].Value,
                    detail: localizer["UnexpectedErrorDeletingIotDevice"].Value,
                    statusCode: StatusCodes.Status500InternalServerError)
            },
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorDeletingIotDevice"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
