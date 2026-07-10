using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a location deletion result.
/// </summary>
public static class ActionResultFromDeleteLocationResultAssembler
{
    /// <summary>
    ///     Converts a delete location result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An empty success response or a localized problem response.</returns>
    public static ActionResult ToActionResultFromDeleteLocationResult(
        Result<DeleteLocationCommand, DeleteLocationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<DeleteLocationCommand, DeleteLocationError>.Success => controller.NoContent(),

            Result<DeleteLocationCommand, DeleteLocationError>.Failure failure =>
                failure.Error switch
                {
                    DeleteLocationError.OrganizationNotFound =>
                        controller.Problem(
                            detail: localizer["DeleteLocationOrganizationNotFound"].Value,
                            statusCode: StatusCodes.Status404NotFound),
                    DeleteLocationError.LocationNotFound =>
                        controller.Problem(
                            detail: localizer["DeleteLocationNotFound"].Value,
                            statusCode: StatusCodes.Status404NotFound),
                    DeleteLocationError.DeleteBlocked =>
                        controller.Problem(
                            detail: localizer["LocationDeleteBlocked"].Value,
                            statusCode: StatusCodes.Status409Conflict),
                    DeleteLocationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorDeletingLocation"].Value,
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
