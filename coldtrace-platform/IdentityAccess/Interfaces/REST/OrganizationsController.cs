using System.Net.Mime;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST;

/// <summary>
///     Organizations controller.
/// </summary>
/// <param name="organizationCommandService">Organization command service.</param>
/// <param name="organizationQueryService">Organization query service.</param>
/// <param name="localizer">String localizer for response messages.</param>
/// <param name="logger">Logger for diagnostics.</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Organizations")]
public class OrganizationsController(
    IOrganizationCommandService organizationCommandService,
    IOrganizationQueryService organizationQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<OrganizationsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets all organizations.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing registered organizations.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all organizations",
        Description = "Gets the organizations registered in the platform",
        OperationId = "GetAllOrganizations")]
    [SwaggerResponse(200, "Organizations found", typeof(IEnumerable<OrganizationResource>))]
    public async Task<ActionResult> GetAllOrganizations(CancellationToken cancellationToken = default)
    {
        var organizations = await organizationQueryService.Handle(new GetAllOrganizationsQuery(), cancellationToken);
        var resources = organizations.Select(OrganizationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Creates an organization.
    /// </summary>
    /// <param name="resource">Organization creation request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the created organization.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates an organization",
        Description = "Creates an organization with the provided legal, commercial, and contact data",
        OperationId = "CreateOrganization")]
    [SwaggerResponse(201, "The organization was created", typeof(OrganizationResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(409, "The organization already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateOrganization(
        [FromBody] CreateOrganizationResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateOrganizationCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await organizationCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateOrganizationResultAssembler.ToActionResultFromCreateOrganizationResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid organization creation request for contact email {ContactEmail}",
                resource.ContactEmail);
            return this.ValidationProblemResponse(localizer, "InvalidOrganizationRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating organization for contact email {ContactEmail}",
                resource.ContactEmail);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingOrganization", 500);
        }
    }
}
