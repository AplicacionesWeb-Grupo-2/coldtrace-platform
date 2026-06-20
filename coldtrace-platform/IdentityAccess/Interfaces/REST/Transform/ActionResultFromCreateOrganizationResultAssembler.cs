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
                        controller.Conflict(localizer["OrganizationContactEmailDuplicated"].Value),

                    CreateOrganizationError.DuplicateTaxId =>
                        controller.Conflict(localizer["OrganizationTaxIdDuplicated"].Value),

                    CreateOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingOrganization"].Value,
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
