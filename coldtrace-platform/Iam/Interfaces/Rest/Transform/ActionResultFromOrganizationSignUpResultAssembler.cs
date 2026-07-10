using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization sign-up result.
/// </summary>
public static class ActionResultFromOrganizationSignUpResultAssembler
{
    /// <summary>
    ///     Converts an organization sign-up result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromOrganizationSignUpResult(
        Result<OrganizationSignUpResult, CreateOrganizationSignUpError> result,
        ControllerBase controller,
        IStringLocalizer<IamMessages> localizer) =>
        result switch
        {
            Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    OrganizationSignUpResourceFromResultAssembler.ToResourceFromResult(success.Value)),

            Result<OrganizationSignUpResult, CreateOrganizationSignUpError>.Failure failure =>
                failure.Error switch
                {
                    CreateOrganizationSignUpError.DuplicateOrganizationContactEmail =>
                        controller.ProblemResponse(localizer, "OrganizationContactEmailDuplicated", StatusCodes.Status409Conflict),

                    CreateOrganizationSignUpError.DuplicateOrganizationTaxId =>
                        controller.ProblemResponse(localizer, "OrganizationTaxIdDuplicated", StatusCodes.Status409Conflict),

                    CreateOrganizationSignUpError.DuplicateUserEmail =>
                        controller.ProblemResponse(localizer, "UserEmailDuplicated", StatusCodes.Status409Conflict),

                    CreateOrganizationSignUpError.InitialRoleNotFound =>
                        controller.ProblemResponse(localizer, "InitialRoleNotFound", 500),

                    CreateOrganizationSignUpError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingOrganizationSignUp", 500),

                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
