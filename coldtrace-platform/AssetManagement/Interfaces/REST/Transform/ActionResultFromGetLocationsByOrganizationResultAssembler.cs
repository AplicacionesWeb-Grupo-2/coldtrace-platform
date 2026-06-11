using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a locations query result.
/// </summary>
public static class ActionResultFromGetLocationsByOrganizationResultAssembler
{
    /// <summary>
    ///     Converts a locations-by-organization result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromGetLocationsByOrganizationResult(
        Result<IEnumerable<Location>, GetLocationsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<Location>, GetLocationsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(LocationResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<Location>, GetLocationsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetLocationsByOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetLocationsByOrganizationError.UnexpectedError =>
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
