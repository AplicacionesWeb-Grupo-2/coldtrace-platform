using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateway update result.
/// </summary>
public static class ActionResultFromUpdateGatewayResultAssembler
{
    /// <summary>
    ///     Converts an update gateway result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromUpdateGatewayResult(
        Result<Gateway, UpdateGatewayError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Gateway, UpdateGatewayError>.Success success =>
                controller.Ok(GatewayResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Gateway, UpdateGatewayError>.Failure failure =>
                failure.Error switch
                {
                    UpdateGatewayError.DuplicateUuid => controller.Conflict(localizer["GatewayUuidDuplicated"].Value),
                    UpdateGatewayError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    UpdateGatewayError.LocationNotFound => controller.NotFound(localizer["LocationNotFound"].Value),
                    UpdateGatewayError.GatewayNotFound => controller.NotFound(localizer["GatewayNotFound"].Value),
                    UpdateGatewayError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorUpdatingGateway"].Value,
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
