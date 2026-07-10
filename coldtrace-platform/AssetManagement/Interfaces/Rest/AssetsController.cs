using ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;
﻿using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.AssetManagement.Application.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.QueryServices;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Interfaces.Rest.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.Rest.Transform;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.AssetManagement.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.Rest;

/// <summary>
///     Assets controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/assets")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Assets")]
public class AssetsController(
    IAssetCommandService assetCommandService,
    IAssetQueryService assetQueryService,
    IStringLocalizer<AssetManagementMessages> localizer,
    ILogger<AssetsController> logger)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets assets by organization",
        Description = "Gets cold-chain assets that belong to the provided organization",
        OperationId = "GetAssetsByOrganization")]
    [SwaggerResponse(200, "Assets found", typeof(IEnumerable<AssetResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetAssetsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await assetQueryService.Handle(
            new GetAssetsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetAssetsByOrganizationResultAssembler
            .ToActionResultFromGetAssetsByOrganizationResult(result, this, localizer);
    }

    [HttpGet("{assetId:int}")]
    [SwaggerOperation(
        Summary = "Gets asset by id",
        Description = "Gets one cold-chain asset that belongs to the provided organization",
        OperationId = "GetAssetById")]
    [SwaggerResponse(200, "Asset found", typeof(AssetResource))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetAssetById(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        CancellationToken cancellationToken = default)
    {
        var result = await assetQueryService.Handle(
            new GetAssetByIdAndOrganizationIdQuery(organizationId, assetId),
            cancellationToken);
        return ActionResultFromGetAssetByIdResultAssembler.ToActionResultFromGetAssetByIdResult(
            result, this, localizer);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates an asset",
        Description = "Creates a cold-chain asset for an organization location",
        OperationId = "CreateAsset")]
    [SwaggerResponse(201, "The asset was created", typeof(AssetResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or location not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Asset UUID already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateAsset(
        [FromRoute] int organizationId,
        [FromBody] CreateAssetResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateAssetCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await assetCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateAssetResultAssembler.ToActionResultFromCreateAssetResult(
                result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid asset creation request for organization {OrganizationId}", organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidAssetRequest");
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating asset for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingAsset", 500);
        }
    }

    [HttpPut("{assetId:int}")]
    [SwaggerOperation(
        Summary = "Updates an asset",
        Description = "Updates a cold-chain asset for an organization location",
        OperationId = "UpdateAsset")]
    [SwaggerResponse(200, "The asset was updated", typeof(AssetResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization, location or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Asset UUID already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateAsset(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        [FromBody] UpdateAssetResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateAssetCommandFromResourceAssembler.ToCommandFromResource(
                resource, organizationId, assetId);
            var result = await assetCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateAssetResultAssembler.ToActionResultFromUpdateAssetResult(
                result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Invalid asset update request for organization {OrganizationId} and asset {AssetId}",
                organizationId, assetId);
            return this.ValidationProblemResponse(localizer, "InvalidAssetRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error while updating asset {AssetId} for organization {OrganizationId}",
                assetId, organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorUpdatingAsset", 500);
        }
    }

    [HttpDelete("{assetId:int}")]
    [SwaggerOperation(
        Summary = "Deletes an asset",
        Description = "Deletes one cold-chain asset that belongs to the provided organization",
        OperationId = "DeleteAsset")]
    [SwaggerResponse(204, "Asset deleted")]
    [SwaggerResponse(400, "Missing or invalid identifier", typeof(ProblemDetails))]
    [SwaggerResponse(404, "Organization or asset not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Asset cannot be deleted because related data exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> DeleteAsset(
        [FromRoute] int organizationId,
        [FromRoute] int assetId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await assetCommandService.Handle(
                new DeleteAssetCommand(organizationId, assetId),
                cancellationToken);
            return ActionResultFromDeleteAssetResultAssembler.ToActionResultFromDeleteAssetResult(
                result, this, localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid asset deletion request for organization {OrganizationId} and asset {AssetId}",
                organizationId,
                assetId);
            return Problem(
                detail: localizer[ex.Message].Value,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while deleting asset {AssetId} for organization {OrganizationId}",
                assetId,
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorDeletingAsset"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
