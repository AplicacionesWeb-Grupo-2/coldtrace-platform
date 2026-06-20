using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

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
        IStringLocalizer<SharedResource> localizer) =>
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
                        controller.Conflict(localizer["OrganizationContactEmailDuplicated"].Value),

                    CreateOrganizationSignUpError.DuplicateOrganizationTaxId =>
                        controller.Conflict(localizer["OrganizationTaxIdDuplicated"].Value),

                    CreateOrganizationSignUpError.DuplicateUserEmail =>
                        controller.Conflict(localizer["UserEmailDuplicated"].Value),

                    CreateOrganizationSignUpError.InitialRoleNotFound =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["InitialRoleNotFound"].Value,
                            statusCode: 500),

                    CreateOrganizationSignUpError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingOrganizationSignUp"].Value,
                            statusCode: 500),

                    _ => controller.Problem(
                        title: localizer["UnexpectedServerError"].Value,
                        detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                        statusCode: 500)
                },

            _ => controller.Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500)
        };
}
