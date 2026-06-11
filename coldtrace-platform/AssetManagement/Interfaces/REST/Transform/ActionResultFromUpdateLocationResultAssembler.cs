using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a location update result.
/// </summary>
public static class ActionResultFromUpdateLocationResultAssembler
{
    /// <summary>
    ///     Converts an update location result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromUpdateLocationResult(
        Result<Location, UpdateLocationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Location, UpdateLocationError>.Success success =>
                controller.Ok(LocationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Location, UpdateLocationError>.Failure failure =>
                failure.Error switch
                {
                    UpdateLocationError.DuplicateName => controller.Conflict(localizer["LocationNameDuplicated"].Value),
                    UpdateLocationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    UpdateLocationError.LocationNotFound => controller.NotFound(localizer["LocationNotFound"].Value),
                    UpdateLocationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorUpdatingLocation"].Value,
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
