using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateway-by-id query result.
/// </summary>
public static class ActionResultFromGetGatewayByIdResultAssembler
{
    /// <summary>
    ///     Converts a gateway-by-id result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromGetGatewayByIdResult(
        Result<Gateway, GetGatewayByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Gateway, GetGatewayByIdAndOrganizationError>.Success success =>
                controller.Ok(GatewayResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Gateway, GetGatewayByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetGatewayByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetGatewayByIdAndOrganizationError.GatewayNotFound =>
                        controller.NotFound(localizer["GatewayNotFound"].Value),
                    GetGatewayByIdAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingGateways"].Value,
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
