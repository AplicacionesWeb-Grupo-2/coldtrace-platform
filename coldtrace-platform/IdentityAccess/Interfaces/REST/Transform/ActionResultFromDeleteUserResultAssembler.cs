using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a user deletion result.
/// </summary>
public static class ActionResultFromDeleteUserResultAssembler
{
    /// <summary>
    ///     Converts a delete user result into an HTTP action result.
    /// </summary>
    public static ActionResult ToActionResultFromDeleteUserResult(
        Result<DeleteUserCommand, DeleteUserError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<DeleteUserCommand, DeleteUserError>.Success => controller.NoContent(),
            Result<DeleteUserCommand, DeleteUserError>.Failure failure => failure.Error switch
            {
                DeleteUserError.OrganizationNotFound => controller.Problem(
                    detail: localizer["OrganizationNotFound"].Value,
                    statusCode: StatusCodes.Status404NotFound),
                DeleteUserError.UserNotFound => controller.Problem(
                    detail: localizer["UserNotFound"].Value,
                    statusCode: StatusCodes.Status404NotFound),
                DeleteUserError.DeleteBlocked => controller.Problem(
                    detail: localizer["UserDeleteBlocked"].Value,
                    statusCode: StatusCodes.Status409Conflict),
                DeleteUserError.UnexpectedError => controller.Problem(
                    title: localizer["UnexpectedServerError"].Value,
                    detail: localizer["UnexpectedErrorDeletingUser"].Value,
                    statusCode: StatusCodes.Status500InternalServerError),
                _ => controller.Problem(
                    title: localizer["UnexpectedServerError"].Value,
                    detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                    statusCode: StatusCodes.Status500InternalServerError)
            },
            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError)
        };
}
