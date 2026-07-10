using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset deletion result.
/// </summary>
public static class ActionResultFromDeleteAssetResultAssembler
{
    public static ActionResult ToActionResultFromDeleteAssetResult(
        Result<DeleteAssetCommand, DeleteAssetError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<DeleteAssetCommand, DeleteAssetError>.Success => controller.NoContent(),

            Result<DeleteAssetCommand, DeleteAssetError>.Failure failure =>
                failure.Error switch
                {
                    DeleteAssetError.OrganizationNotFound =>
                        controller.Problem(
                            detail: localizer["AssetDeleteOrganizationNotFound"].Value,
                            statusCode: StatusCodes.Status404NotFound),
                    DeleteAssetError.AssetNotFound =>
                        controller.Problem(
                            detail: localizer["AssetDeleteNotFound"].Value,
                            statusCode: StatusCodes.Status404NotFound),
                    DeleteAssetError.DeleteBlocked =>
                        controller.Problem(
                            detail: localizer["AssetDeleteBlocked"].Value,
                            statusCode: StatusCodes.Status409Conflict),
                    DeleteAssetError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorDeletingAsset"].Value,
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
