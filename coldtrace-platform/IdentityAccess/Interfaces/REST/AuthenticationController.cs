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
[AllowAnonymous]
public class AuthenticationController(
    IUserCommandService userCommandService,
    ISocialAuthenticationCommandService socialAuthenticationCommandService,
    ISocialIdentityProfileCommandService socialIdentityProfileCommandService,
    ISocialOrganizationSignUpCommandService socialOrganizationSignUpCommandService,
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

    /// <summary>
    ///     Validates a Google or Apple response and issues a ColdTrace JWT for a linked user.
    /// </summary>
    [HttpPost("social/{provider}/token-exchange")]
    [SwaggerOperation(
        Summary = "Social provider sign-in",
        Description = "Validates a Google or Apple OIDC response server-side, links it to a local user, and returns a ColdTrace JWT",
        OperationId = "SocialSignIn")]
    [SwaggerResponse(200, "User authenticated successfully", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(401, "Provider validation failed", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(422, "Social identity requires onboarding", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(503, "Provider configuration is missing", typeof(AuthenticationErrorResource))]
    public async Task<ActionResult> SocialSignIn(
        [FromRoute] string provider,
        [FromBody] SocialTokenExchangeResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SocialSignInCommandFromResourceAssembler.ToCommandFromResource(provider, resource);
            var result = await socialAuthenticationCommandService.Handle(command, cancellationToken);
            return ActionResultFromSocialAuthenticationResultAssembler.ToAuthenticatedUserActionResult(result, this);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid social sign-in request for provider {Provider}", provider);
            return BadRequest(new AuthenticationErrorResource(
                "VALIDATION_ERROR",
                "Request validation failed",
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected social sign-in error for provider {Provider}", provider);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new AuthenticationErrorResource(
                    "UNEXPECTED_ERROR",
                    "Unexpected error",
                    "identity-access.authentication.error.unexpected"));
        }
    }

    /// <summary>
    ///     Validates provider profile data for organization onboarding.
    /// </summary>
    [HttpPost("social/{provider}/profile-preview")]
    [SwaggerOperation(
        Summary = "Social provider profile preview",
        Description = "Validates a Google or Apple OIDC response and returns verified profile hints",
        OperationId = "SocialProfilePreview")]
    [SwaggerResponse(200, "Social profile validated successfully", typeof(SocialIdentityProfileResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(401, "Provider validation failed", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(503, "Provider configuration is missing", typeof(AuthenticationErrorResource))]
    public async Task<ActionResult> SocialProfilePreview(
        [FromRoute] string provider,
        [FromBody] SocialTokenExchangeResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SocialSignInCommandFromResourceAssembler.ToCommandFromResource(provider, resource);
            var result = await socialIdentityProfileCommandService.Handle(command, cancellationToken);
            return ActionResultFromSocialAuthenticationResultAssembler.ToProfileActionResult(result, this);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid social profile request for provider {Provider}", provider);
            return BadRequest(new AuthenticationErrorResource(
                "VALIDATION_ERROR",
                "Request validation failed",
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected social profile error for provider {Provider}", provider);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new AuthenticationErrorResource(
                    "UNEXPECTED_ERROR",
                    "Unexpected error",
                    "identity-access.authentication.error.unexpected"));
        }
    }

    /// <summary>
    ///     Creates an organization and its first user from a verified social identity.
    /// </summary>
    [HttpPost("social/{provider}/organization-sign-up")]
    [SwaggerOperation(
        Summary = "Social provider organization sign-up",
        Description = "Validates a provider identity, creates the organization and first user, links the identity, and returns a ColdTrace JWT",
        OperationId = "SocialOrganizationSignUp")]
    [SwaggerResponse(200, "Organization sign-up completed", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(401, "Provider validation failed", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(409, "Provider identity or organization already exists", typeof(AuthenticationErrorResource))]
    [SwaggerResponse(503, "Provider configuration is missing", typeof(AuthenticationErrorResource))]
    public async Task<ActionResult> SocialOrganizationSignUp(
        [FromRoute] string provider,
        [FromBody] SocialOrganizationSignUpResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = SocialOrganizationSignUpCommandFromResourceAssembler.ToCommandFromResource(
                provider,
                resource);
            var result = await socialOrganizationSignUpCommandService.Handle(command, cancellationToken);
            return ActionResultFromSocialAuthenticationResultAssembler.ToAuthenticatedUserActionResult(result, this);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid social organization sign-up for provider {Provider}", provider);
            return BadRequest(new AuthenticationErrorResource(
                "VALIDATION_ERROR",
                "Request validation failed",
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected social organization sign-up error for provider {Provider}", provider);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new AuthenticationErrorResource(
                    "UNEXPECTED_ERROR",
                    "Unexpected error",
                    "identity-access.authentication.error.unexpected"));
        }
    }
}
