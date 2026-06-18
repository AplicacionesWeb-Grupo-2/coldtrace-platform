using ColdTrace.Platform.Shared.Domain.Model.Entities;

namespace ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;

/// <summary>
///     Audit extension for the organization aggregate.
/// </summary>
public partial class Organization : IAuditableEntity
{
    /// <summary>
    ///     Gets or sets the timestamp when this organization was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp when this organization was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
