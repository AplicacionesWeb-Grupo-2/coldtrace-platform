using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
﻿using ColdTrace.Platform.AssetManagement.Domain.Model.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from an asset settings by organization query result.
/// </summary>
public static class ActionResultFromGetAssetSettingsByOrganizationResultAssembler
{
    public static ActionResult ToActionResultFromGetAssetSettingsByOrganizationResult(
        Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError> result,
        ControllerBase controller,
        IStringLocalizer<AssetManagementMessages> localizer) =>
        result switch
        {
            Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Success success =>
                controller.Ok(success.Value.Select(
                    AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity)),

            Result<IEnumerable<AssetSettings>, GetAssetSettingsByOrganizationError>.Failure failure =>
                failure.Error switch
                {
                    GetAssetSettingsByOrganizationError.OrganizationNotFound =>
                        controller.ProblemResponse(localizer, "OrganizationNotFound", StatusCodes.Status404NotFound),
                    GetAssetSettingsByOrganizationError.UnexpectedError =>
                        controller.ProblemResponse(localizer, "UnexpectedErrorGettingAssetSettings", 500),
                    _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
                },

            _ => controller.ProblemResponse(localizer, "UnexpectedErrorProcessingRequest", 500, RestErrorCodes.UnexpectedError)
        };
}