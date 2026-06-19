namespace ColdTrace.Platform.Alerts.Domain.Model.Queries;

/// <summary>
///     Query for retrieving notification read models owned by an organization.
/// </summary>
public record GetNotificationsByOrganizationIdQuery(int OrganizationId);
