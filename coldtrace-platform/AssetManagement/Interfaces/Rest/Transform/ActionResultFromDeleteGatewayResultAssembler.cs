using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a gateway deletion result.
/// </summary>
public static class ActionResultFromDeleteGatewayResultAssembler
{
    /// <summary>
    ///     Converts a delete gateway result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <param name="localizer">The string localizer used for response messages.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromDeleteGatewayResult(
        Result<DeleteGatewayCommand, DeleteGatewayError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<DeleteGatewayCommand, DeleteGatewayError>.Success => controller.NoContent(),

            Result<DeleteGatewayCommand, DeleteGatewayError>.Failure failure =>
                failure.Error switch
                {
                    DeleteGatewayError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    DeleteGatewayError.GatewayNotFound => controller.NotFound(localizer["GatewayNotFound"].Value),
                    DeleteGatewayError.DeleteBlocked => controller.Conflict(localizer["GatewayDeleteBlocked"].Value),
                    DeleteGatewayError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorDeletingGateway"].Value,
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
