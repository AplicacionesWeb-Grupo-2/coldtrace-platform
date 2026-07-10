namespace ColdTrace.Platform.Iam.Interfaces.Acl;

/// <summary>
///     Contract exposed by IAM to other bounded contexts.
/// </summary>
public interface IIamContextFacade
{
    /// <summary>
    ///     Determines whether an organization exists.
    /// </summary>
    Task<bool> OrganizationExistsAsync(int organizationId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists organization identifiers without exposing IAM aggregates.
    /// </summary>
    Task<IEnumerable<int>> ListOrganizationIdsAsync(CancellationToken cancellationToken = default);
}
