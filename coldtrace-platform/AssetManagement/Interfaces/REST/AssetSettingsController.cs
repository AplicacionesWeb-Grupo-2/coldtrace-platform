using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST;

/// <summary>
///     Asset settings controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Asset Settings")]
public class AssetSettingsController(
    IAssetSettingsCommandService assetSettingsCommandService,
    IAssetSettingsQueryService assetSettingsQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<AssetSettingsController> logger)
    : ControllerBase
{
    [HttpGet("asset-settings")]
    [SwaggerOperation(
        Summary = "Gets asset settings by organization",
        Description = "Gets default and asset-specific settings that belong to the provided organization",
        OperationId = "GetAssetSettingsByOrganization")]
    [SwaggerResponse(200, "Settings found", typeof(IEnumerable<AssetSettingsResource>))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetAssetSettingsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var settings = await assetSettingsQueryService.Handle(
            new GetAssetSettingsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return Ok(settings.Select(AssetSettingsResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("assets/{assetId:int}/settings")]
    [SwaggerOperation(
        Summary = "Gets effective asset settings",
        Description = "Gets asset-specific settings or organization default settings for one asset",
        OperationId = "GetEffectiveAssetSettings")]
    [SwaggerResponse(200, "Settings found", typeof(AssetSettingsResource))]
    [SwaggerResponse(404, "Asset or settings not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetEffectiveAssetSettings(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        CancellationToken cancellationToken = default)
    {
        var result = await assetSettingsQueryService.Handle(
            new GetEffectiveAssetSettingsByAssetIdQuery(organizationId, assetId),
            cancellationToken);
        return ActionResultFromGetEffectiveAssetSettingsResultAssembler
            .ToActionResultFromGetEffectiveAssetSettingsResult(result, this, localizer);
    }

    [HttpPut("asset-settings/default")]
    [SwaggerOperation(
        Summary = "Saves default asset settings",
        Description = "Creates or updates default safety and telemetry settings for an organization",
        OperationId = "SaveDefaultAssetSettings")]
    [SwaggerResponse(200, "Default settings saved", typeof(AssetSettingsResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SaveDefaultAssetSettings(
        [FromRoute] int organizationId,
        [FromBody] SaveAssetSettingsResource resource,
        CancellationToken cancellationToken = default)
    {
        return await SaveAssetSettingsInternal(organizationId, null, resource, cancellationToken);
    }

    [HttpPut("assets/{assetId:int}/settings")]
    [SwaggerOperation(
        Summary = "Saves settings for an asset",
        Description = "Creates or updates safety and telemetry settings for one organization asset",
        OperationId = "SaveAssetSettings")]
    [SwaggerResponse(200, "Asset settings saved", typeof(AssetSettingsResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SaveAssetSettings(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        [FromBody] SaveAssetSettingsResource resource,
        CancellationToken cancellationToken = default)
    {
        return await SaveAssetSettingsInternal(organizationId, assetId, resource, cancellationToken);
    }

    private async Task<ActionResult> SaveAssetSettingsInternal(
        int organizationId,
        int? assetId,
        SaveAssetSettingsResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = SaveAssetSettingsCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                assetId);
            var result = await assetSettingsCommandService.Handle(command, cancellationToken);
            return ActionResultFromSaveAssetSettingsResultAssembler
                .ToActionResultFromSaveAssetSettingsResult(result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid asset settings request for organization {OrganizationId}", organizationId);
            return BadRequest(localizer["InvalidRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error saving asset settings for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: 500);
        }
    }
}
