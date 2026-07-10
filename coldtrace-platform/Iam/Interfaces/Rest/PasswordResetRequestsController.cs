using System.Net.Mime;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;
using ColdTrace.Platform.Iam.Interfaces.Rest.Transform;
using ColdTrace.Platform.Iam.Resources;
using ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest;

/// <summary>
///     Password reset request controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Password Reset Requests")]
[AllowAnonymous]
public class PasswordResetRequestsController(
    IPasswordResetRequestCommandService passwordResetRequestCommandService,
    IStringLocalizer<IamMessages> localizer,
    ILogger<PasswordResetRequestsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Accepts a password reset request without revealing whether the email exists.
    /// </summary>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [SwaggerOperation(
        Summary = "Requests a password reset",
        Description = "Accepts a password reset request and stores safe token metadata for a matching ColdTrace user",
        OperationId = "CreatePasswordResetRequest")]
    [SwaggerResponse(202, "Password reset request accepted", typeof(PasswordResetRequestResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(ValidationProblemDetails))]
    [SwaggerResponse(500, "Password reset request could not be prepared", typeof(ProblemDetails))]
    public async Task<ActionResult> CreatePasswordResetRequest(
        [FromBody] CreatePasswordResetRequestResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreatePasswordResetRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await passwordResetRequestCommandService.Handle(command, cancellationToken);
            return ActionResultFromPasswordResetRequestResultAssembler.ToActionResultFromPasswordResetRequestResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid password reset request");
            return BadRequest(localizer["InvalidPasswordResetRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while requesting a password reset");
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["PasswordResetRequestFailed"].Value,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
