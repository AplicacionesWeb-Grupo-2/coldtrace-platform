using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Domain.Model.Aggregates;
using ColdTrace.Platform.Iam.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a user creation result.
/// </summary>
public static class ActionResultFromCreateUserResultAssembler
{
    /// <summary>
    ///     Converts a create user result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromCreateUserResult(
        Result<User, CreateUserError> result,
        ControllerBase controller,
        IStringLocalizer<IamMessages> localizer) =>
        result switch
        {
            Result<User, CreateUserError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<User, CreateUserError>.Failure failure =>
                failure.Error switch
                {
                    CreateUserError.DuplicateEmail => controller.ProblemResponse(localizer, "UserEmailDuplicated", StatusCodes.Status409Conflict),
                    CreateUserError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    CreateUserError.RoleNotFound => controller.ProblemResponse(localizer, "RoleNotFound", StatusCodes.Status404NotFound),
                    CreateUserError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingUser", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
