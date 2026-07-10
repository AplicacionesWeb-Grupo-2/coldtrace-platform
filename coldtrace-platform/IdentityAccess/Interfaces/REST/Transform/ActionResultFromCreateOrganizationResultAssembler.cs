using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an organization creation result.
/// </summary>
public static class ActionResultFromCreateOrganizationResultAssembler
{
    /// <summary>
    ///     Converts a create organization result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromCreateOrganizationResult(
        Result<Organization, CreateOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Organization, CreateOrganizationError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    OrganizationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Organization, CreateOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    CreateOrganizationError.DuplicateContactEmail =>
                        controller.ProblemResponse(localizer, "OrganizationContactEmailDuplicated", StatusCodes.Status409Conflict),

                    CreateOrganizationError.DuplicateTaxId =>
                        controller.ProblemResponse(localizer, "OrganizationTaxIdDuplicated", StatusCodes.Status409Conflict),

                    CreateOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorCreatingOrganization", 500),

                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}
