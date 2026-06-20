using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from an effective asset settings query result.
/// </summary>
public static class ActionResultFromGetEffectiveAssetSettingsByAssetResultAssembler
{
    public static ActionResult ToActionResultFromGetEffectiveAssetSettingsByAssetResult(
        Result<AssetSettings, GetEffectiveAssetSettingsByAssetError> result,
        ControllerBase controller,
        IStringLocalizer<SharedResource> localizer) =>
        result switch
        {
            Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Success success =>
                controller.Ok(AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure failure =>
                failure.Error switch
                {
                    GetEffectiveAssetSettingsByAssetError.OrganizationNotFound =>
                        controller.NotFound(localizer["OrganizationNotFound"].Value),
                    GetEffectiveAssetSettingsByAssetError.AssetNotFound =>
                        controller.NotFound(localizer["AssetNotFound"].Value),
                    GetEffectiveAssetSettingsByAssetError.UnexpectedError =>
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