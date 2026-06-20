using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset settings by organization query result.
/// </summary>
public static class ActionResultFromGetAssetSettingsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetAssetSettingsByOrganizationResult(
        Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(
                    AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetAssetSettingsByOrganizationError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetAssetSettingsByOrganizationError.UnexpectedError =>
                        controller.Problem(
                            title: localizer["UnexpectedServerError"].Value,
                            detail: localizer["UnexpectedErrorGettingAssetSettings"].Value,
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