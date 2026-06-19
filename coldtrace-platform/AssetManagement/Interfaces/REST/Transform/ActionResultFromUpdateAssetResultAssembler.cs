using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset update result.
/// </summary>
public static class ActionResultFromUpdateAssetResultAssembler
{
    public static ActionResult ToActionResultFromUpdateAssetResult(
        Result<Asset, UpdateAssetError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Asset, UpdateAssetError>.Success success =>
                controller.Ok(AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, UpdateAssetError>.Failure failure =>
                failure.Error switch
                {
                    UpdateAssetError.DuplicateUuid =>
                        controller.Conflict(localizer["AssetUuidDuplicated"].Value),
                    UpdateAssetError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    UpdateAssetError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    UpdateAssetError.LocationNotFound =>
                        controller.NotFound(localizer["LocationNotFound"].Value),
                    UpdateAssetError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorUpdatingAsset"].Value,
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