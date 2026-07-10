namespace ColdTrace.Platform.Iam.Domain.Model.Queries;

/// <summary>
///     Query for getting users by organization.
/// </summary>
/// <param name="OrganizationId">Organization identifier.</param>
public record GetUsersByOrganizationIdQuery(int OrganizationId);
