using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset-by-id query result.
/// </summary>
public static class ActionResultFromGetAssetByIdResultAssembler
{
    public static ActionResult ToActionResultFromGetAssetByIdResult(
        Result<Asset, GetAssetByIdAndOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<Asset, GetAssetByIdAndOrganizationError>.Success success =>
                controller.Ok(AssetResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<Asset, GetAssetByIdAndOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetAssetByIdAndOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetAssetByIdAndOrganizationError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    GetAssetByIdAndOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingAssets"].Value,
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