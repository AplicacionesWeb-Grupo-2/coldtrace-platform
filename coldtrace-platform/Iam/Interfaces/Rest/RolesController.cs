using System.Net.Mime;
using ColdTrace.Platform.Iam.Application.CommandServices;
using ColdTrace.Platform.Iam.Application.QueryServices;
using ColdTrace.Platform.Iam.Domain.Model.Queries;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;
using ColdTrace.Platform.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Iam.Interfaces.Rest;

/// <summary>
///     Roles controller.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Roles")]
public class RolesController(IRoleQueryService roleQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets all roles and their permissions.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A response containing role resources.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets all roles",
        Description = "Gets all roles and their permissions",
        OperationId = "GetAllRoles")]
    [SwaggerResponse(200, "Roles found", typeof(IEnumerable<RoleResource>))]
    public async Task<ActionResult> GetAllRoles(CancellationToken cancellationToken = default)
    {
        var roles = await roleQueryService.Handle(new GetAllRolesQuery(), cancellationToken);
        var resources = roles.Select(RoleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
