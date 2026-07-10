using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
﻿using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an effective asset settings query result.
/// </summary>
public static class ActionResultFromGetEffectiveAssetSettingsByAssetResultAssembler
{
    public static ActionResult ToActionResultFromGetEffectiveAssetSettingsByAssetResult(
        Result<AssetSettings, GetEffectiveAssetSettingsByAssetError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Success success =>
                controller.Ok(AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),

            Result<AssetSettings, GetEffectiveAssetSettingsByAssetError>.Failure failure =>
                failure.Error switch
                {
                    GetEffectiveAssetSettingsByAssetError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetEffectiveAssetSettingsByAssetError.AssetNotFound =>
                        controller.ProblemResponse(localizer, "AssetNotFound", StatusCodes.Status404NotFound),
                    GetEffectiveAssetSettingsByAssetError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingAssetSettings", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}