using ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

namespace ColdTrace.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     Role aggregate for authorization metadata used by the frontend.
/// </summary>
public class Role
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Role()
    {
        Name = string.Empty;
        Label = string.Empty;
        Permissions = [];
    }

    /// <summary>
    ///     Creates a role with a display label and permission set.
    /// </summary>
    /// <param name="name">Stable role name.</param>
    /// <param name="label">Display label for the role.</param>
    /// <param name="permissions">Permissions assigned to the role.</param>
    public Role(string name, string label, IEnumerable<Permission> permissions)
    {
        Name = name.Trim();
        Label = label.Trim();
        Permissions = [..permissions];
    }

    /// <summary>
    ///     Gets the role identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the stable role name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the role display label.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    ///     Gets permissions assigned to the role.
    /// </summary>
    public ICollection<Permission> Permissions { get; private set; }
}
