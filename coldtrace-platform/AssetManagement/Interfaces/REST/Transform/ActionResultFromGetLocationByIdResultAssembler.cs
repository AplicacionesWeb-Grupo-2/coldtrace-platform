using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a location-by-id query result.
/// </summary>
public static class ActionResultFromGetLocationByIdResultAssembler
{
    /// <summary>
    ///     Converts a location-by-id result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromGetLocationByIdResult(
        Result<Location, GetLocationByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Location, GetLocationByIdAndOrganizationError>.Success success =>
                controller.Ok(LocationResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Location, GetLocationByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetLocationByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetLocationByIdAndOrganizationError.LocationNotFound =>
                        controller.NotFound(localizer["LocationNotFound"].Value),
                    GetLocationByIdAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingLocations"].Value,
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
