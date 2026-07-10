using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a user role assignment result.
/// </summary>
public static class ActionResultFromAssignUserRoleResultAssembler
{
    /// <summary>
    ///     Converts an assign user role result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromAssignUserRoleResult(
        Result<User, AssignUserRoleError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<User, AssignUserRoleError>.Success success =>
                controller.Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<User, AssignUserRoleError>.Failure failure =>
                failure.Error switch
                {
                    AssignUserRoleError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    AssignUserRoleError.UserNotFound => controller.ProblemResponse(localizer, "UserNotFound", StatusCodes.Status404NotFound),
                    AssignUserRoleError.RoleNotFound => controller.ProblemResponse(localizer, "RoleNotFound", StatusCodes.Status404NotFound),
                    AssignUserRoleError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorAssigningUserRole", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
