using System.Net.Mime;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST;

/// <summary>
///     Organization sign-ups controller.
/// </summary>
/// <param name="organizationSignUpCommandService">Organization sign-up command service.</param>
/// <param name="localizer">String localizer for response messages.</param>
/// <param name="logger">Logger for diagnostics.</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Organization Sign-Ups")]
public class OrganizationSignUpsController(
    IOrganizationSignUpCommandService organizationSignUpCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<OrganizationSignUpsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Signs up an organization and its first user.
    /// </summary>
    /// <param name="resource">Organization sign-up request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the created organization and first user.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Signs up an organization",
        Description = "Creates an organization and its first super administrator user in one transaction",
        OperationId = "CreateOrganizationSignUp")]
    [SwaggerResponse(201, "The organization sign-up was completed", typeof(OrganizationSignUpResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(409, "The organization or user already exists", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateOrganizationSignUp(
        [FromBody] CreateOrganizationSignUpResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateOrganizationSignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await organizationSignUpCommandService.Handle(command, cancellationToken);
            return ActionResultFromOrganizationSignUpResultAssembler.ToActionResultFromOrganizationSignUpResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid organization sign-up request for contact email {ContactEmail}",
                resource.ContactEmail);
            return this.ValidationProblemResponse(localizer, "InvalidOrganizationSignUpRequest");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while signing up organization for contact email {ContactEmail}",
                resource.ContactEmail);
            return this.ProblemResponse(localizer, "UnexpectedErrorCreatingOrganizationSignUp", 500);
        }
    }
}
