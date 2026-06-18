using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset creation result.
/// </summary>
public static class ActionResultFromCreateAssetResultAssembler
{
    public static ActionResult ToActionResultFromCreateAssetResult(
        Result<Asset, CreateAssetError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Asset, CreateAssetError>.Success success =>
                controller.StatusCode(
                    StatusCodes.Status201Created,
                    AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, CreateAssetError>.Failure failure =>
                failure.Error switch
                {
                    CreateAssetError.DuplicateUuid =>
                        controller.Conflict(localizer["AssetUuidDuplicated"].Value),
                    CreateAssetError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    CreateAssetError.LocationNotFound =>
                        controller.NotFound(localizer["LocationNotFound"].Value),
                    CreateAssetError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorCreatingAsset"].Value,
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