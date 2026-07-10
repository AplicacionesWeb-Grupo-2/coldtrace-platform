using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a password reset request result.
/// </summary>
public static class ActionResultFromPasswordResetRequestResultAssembler
{
    /// <summary>
    ///     Converts a password reset request result into an HTTP response.
    /// </summary>
    public static ActionResult ToActionResultFromPasswordResetRequestResult(
        Result<PasswordResetRequestResult, CreatePasswordResetRequestError> result,
        ControllerBase controller,
        IStringLocalizer<IamMessages> localizer) =>
        result switch
        {
            Result<PasswordResetRequestResult, CreatePasswordResetRequestError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status202Accepted,
                    PasswordResetRequestResourceFromResultAssembler.ToResourceFromResult(success.Value)),
            Result<PasswordResetRequestResult, CreatePasswordResetRequestError>.Failure =>
                controller.Problem(
                    title: localizer["UnexpectedServerError"].Value,
                    detail: localizer["PasswordResetRequestFailed"].Value,
                    statusCode: StatusCodes.Status500InternalServerError),
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };
}
