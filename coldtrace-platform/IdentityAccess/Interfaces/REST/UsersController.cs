using System.Net.Mime;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Queries;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using ColdTrace.Platform.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST;

/// <summary>
///     Users controller.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/users")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Users")]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<UsersController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Gets users that belong to an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing user resources.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets users by organization",
        Description = "Gets users that belong to the provided organization",
        OperationId = "GetUsersByOrganization")]
    [SwaggerResponse(200, "Users found", typeof(IEnumerable<UserResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetUsersByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await userQueryService.Handle(
            new GetUsersByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetUsersByOrganizationResultAssembler.ToActionResultFromGetUsersByOrganizationResult(
            result,
            this,
            localizer);
    }

    /// <summary>
    ///     Creates a user under an organization.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="resource">User creation request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the created user resource.</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a user",
        Description = "Creates a user linked to an organization and role",
        OperationId = "CreateUser")]
    [SwaggerResponse(201, "The user was created", typeof(UserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization or role not found", typeof(string))]
    [SwaggerResponse(409, "User email already exists", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> CreateUser(
        [FromRoute] int organizationId,
        [FromBody] CreateUserResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource, organizationId);
            var result = await userCommandService.Handle(command, cancellationToken);
            return ActionResultFromCreateUserResultAssembler.ToActionResultFromCreateUserResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid user creation request for organization {OrganizationId}", organizationId);
            return BadRequest(localizer["InvalidUserRequest"].Value);
        }
        catch (PlanLimitExceededException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating user for organization {OrganizationId}",
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorCreatingUser"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Assigns a role to an existing organization user.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <param name="resource">User role assignment request resource.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing the updated user resource.</returns>
    [HttpPatch("{userId:int}/role")]
    [SwaggerOperation(
        Summary = "Assigns a user role",
        Description = "Assigns an existing role to an existing user inside the provided organization",
        OperationId = "AssignUserRole")]
    [SwaggerResponse(200, "The user role was assigned", typeof(UserResource))]
    [SwaggerResponse(400, "The request payload is invalid", typeof(string))]
    [SwaggerResponse(404, "Organization, user or role not found", typeof(string))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> AssignUserRole(
        [FromRoute] int organizationId,
        [FromRoute] int userId,
        [FromBody] AssignUserRoleResource resource,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = AssignUserRoleCommandFromResourceAssembler.ToCommandFromResource(
                resource,
                organizationId,
                userId);
            var result = await userCommandService.Handle(command, cancellationToken);
            return ActionResultFromAssignUserRoleResultAssembler.ToActionResultFromAssignUserRoleResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid user role assignment request for organization {OrganizationId} and user {UserId}",
                organizationId,
                userId);
            return BadRequest(localizer["InvalidUserRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while assigning a role to user {UserId} for organization {OrganizationId}",
                userId,
                organizationId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorAssigningUserRole"].Value,
                statusCode: 500);
        }
    }

    /// <summary>
    ///     Deletes an existing organization user.
    /// </summary>
    /// <param name="organizationId">Organization identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An empty response on success or a problem detail on failure.</returns>
    [HttpDelete("{userId:int}")]
    [SwaggerOperation(
        Summary = "Deletes a user",
        Description = "Deletes one user that belongs to the provided organization",
        OperationId = "DeleteUser")]
    [SwaggerResponse(204, "The user was deleted")]
    [SwaggerResponse(400, "An identifier is invalid", typeof(ProblemDetails))]
    [SwaggerResponse(404, "Organization or user not found", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Related data prevents deletion", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> DeleteUser(
        [FromRoute] int organizationId,
        [FromRoute] int userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await userCommandService.Handle(
                new DeleteUserCommand(organizationId, userId),
                cancellationToken);
            return ActionResultFromDeleteUserResultAssembler.ToActionResultFromDeleteUserResult(
                result,
                this,
                localizer);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(
                ex,
                "Invalid user deletion request for organization {OrganizationId} and user {UserId}",
                organizationId,
                userId);
            return Problem(
                detail: localizer["InvalidUserRequest"].Value,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
