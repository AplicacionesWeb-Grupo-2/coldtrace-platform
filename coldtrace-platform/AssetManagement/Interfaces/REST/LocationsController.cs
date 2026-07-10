using System.Net.Mime;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Model.Queries;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Resources;
using ColdTrace.Platform.AssetManagement.Interfaces.REST.Transform;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.AssetManagement.Interfaces.REST;

/// <summary>
///     Locations controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/locations")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Locations")]
public class LocationsController(
    ILocationCommandService locationCommandService,
    ILocationQueryService locationQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<LocationsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets locations that belong to an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing location resources.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets locations by organization",
        Description = "Gets operational locations that belong to the provided organization",
        OperationId = "GetLocationsByOrganization")]
    [SwaggerResponse(200, "Locations found", typeof(IEnumerable<LocationResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetLocationsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await locationQueryService.Handle(
            new GetLocationsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetLocationsByOrganizationResultAssembler
            .ToActionResultFromGetLocationsByOrganizationResult(
                result,
                this,
                localizer);
    }

    /// <summary>
    ///     Gets one location by id.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="locationId">Location identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing one location resource.</returns>
    [HttpGet("{locationId:int}")]
    [SwaggerOperation(
        Summary = "Gets location by id",
        Description = "Gets one operational location that belongs to the provided organization",
        OperationId = "GetLocationById")]
    [SwaggerResponse(200, "Location found", typeof(LocationResource))]
    [SwaggerResponse(404, "Organization or location not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetLocationById(
        [FromRoute] int organizationId,
        [FromRoute] int locationId,
        CancellationToken cancellationToken = default)
    {
        var result = await locationQueryService.Handle(
            new GetLocationByIdAndOrganizationIdQuery(organizationId, locationId),
            cancellationToken);
        return ActionResultFromGetLocationByIdResultAssembler.ToActionResultFromGetLocationByIdResult(
            result,
            this,
            localizer);
    }

    /// <summary>
    ///     Creates a location under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="resource">Location creation request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the created location resource.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a location",
        Description = "Creates an operational location for an organization",
        OperationId = "CreateLocation")]
    [SwaggerResponse(201, "The location was created", typeof(LocationResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Location name already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateLocation(
        [FromRoute] int organizationId,
        [FromBody] CreateLocationResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateLocationCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await locationCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateLocationResultAssembler.ToActionResultFromCreateLocationResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid location creation request for organization {OrganizationId}",
                organizationId);
            return this.ValidationProblemResponse(localizer, "InvalidLocationRequest");
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while creating location for organization {OrganizationId}",
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingLocation", 500);
        }
    }

    /// <summary>
    ///     Updates a location under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="locationId">Location identifier.</param>
    /// <param name="resource">Location update request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the updated location resource.</returns>
    [HttpPut("{locationId:int}")]
    [SwaggerOperation(
        Summary = "Updates a location",
        Description = "Updates an operational location for an organization",
        OperationId = "UpdateLocation")]
    [SwaggerResponse(200, "The location was updated", typeof(LocationResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(404, "Organization or location not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Location name already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> UpdateLocation(
        [FromRoute] int organizationId,
        [FromRoute] int locationId,
        [FromBody] UpdateLocationResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = UpdateLocationCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                locationId);
            var result = await locationCommandService.Handle(command, cancellationToken);
            return ActionResultFromUpdateLocationResultAssembler.ToActionResultFromUpdateLocationResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid location update request for organization {OrganizationId} and location {LocationId}",
                organizationId,
                locationId);
            return this.ValidationProblemResponse(localizer, "InvalidLocationRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while updating location {LocationId} for organization {OrganizationId}",
                locationId,
                organizationId);
            return this.ProblemResponse(localizer, "UnexpectedErrorUpdatingLocation", 500);
        }
    }
}
