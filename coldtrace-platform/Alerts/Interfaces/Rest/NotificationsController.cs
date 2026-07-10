using System.Net.Mime;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Alerts.Application.CommandServices;
using ColdTrace.Platform.Alerts.Application.QueryServices;
using ColdTrace.Platform.Alerts.Interfaces.Rest.Resources;
using ColdTrace.Platform.Alerts.Interfaces.Rest.Transform;
using ColdTrace.Platform.Alerts.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.Alerts.Interfaces.Rest;

/// <summary>
///     REST controller exposing notification read model endpoints.
/// </summary>
[ApiController]
[Route("api/v1/organizations/{organizationId:int}/notifications")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Notifications")]
public class NotificationsController(
    INotificationQueryService notificationQueryService,
    IStringLocalizer<AlertsMessages> localizer)
    : ControllerBase
{
    /// <summary>
    ///     Gets notifications for an organization.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets notifications by organization",
        Description = "Gets incident notification read models owned by the provided organization",
        OperationId = "GetNotificationsByOrganization")]
    [SwaggerResponse(200, "Notifications found", typeof(IEnumerable<NotificationResource>))]
    [SwaggerResponse(404, "Organization not found", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Unexpected server error", typeof(ProblemDetails))]
    public async Task<ActionResult> GetNotificationsByOrganizationId(
        [FromRoute] int organizationId,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationQueryService.Handle(
            new GetNotificationsByOrganizationIdQuery(organizationId),
            cancellationToken);
        return ActionResultFromGetNotificationsByOrganizationResultAssembler
            .ToActionResultFromGetNotificationsByOrganizationResult(result, this, localizer);
    }
}
