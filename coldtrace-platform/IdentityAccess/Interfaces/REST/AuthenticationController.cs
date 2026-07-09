using System.Net.Mime;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST;

/// <summary>
///     Authentication controller.
/// </summary>
[ApiController]
[Route("api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<AuthenticationController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Authenticates a user and issues a JWT.
    /// </summary>
    /// <param name="resource">Sign-in request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The authenticated user resource with a JWT.</returns>
    [AllowAnonymous] // Public credential exchange; every other controller action uses the fallback policy.
    [HttpPost("sign-in")]
    [SwaggerOperation(
        Summary = "User sign-in",
        Description = "Authenticates an organization user with email and password",
        OperationId = "SignIn")]
    [SwaggerResponse(200, "User authenticated successfully", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "Invalid credentials", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> SignIn(
        [FromBody] SignInResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await userCommandService.Handle(command, cancellationToken);
            return ActionResultFromSignInResultAssembler.ToActionResultFromSignInResult(result, this);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid sign-in request for email {Email}", resource.Email);
            return BadRequest(new AuthenticationErrorResource(
                "VALIDATION_ERROR",
                "Request validation failed",
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while authenticating email {Email}", resource.Email);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorProcessingRequest"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
