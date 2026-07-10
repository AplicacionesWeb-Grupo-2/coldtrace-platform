namespace ColdTrace.Platform.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Permission value object embedded in a role.
/// </summary>
public class Permission
{
    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Permission()
    {
        Resource = string.Empty;
        Action = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    ///     Creates a permission value object.
    /// </summary>
    /// <param name="id">Permission identifier used by the frontend resource contract.</param>
    /// <param name="resource">Protected resource name.</param>
    /// <param name="action">Action allowed over the resource.</param>
    /// <param name="description">Translation key used as permission description.</param>
    public Permission(int id, string resource, string action, string description)
    {
        Id = id;
        Resource = resource.Trim();
        Action = action.Trim();
        Description = description.Trim();
    }

    /// <summary>
    ///     Gets the permission identifier.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the protected resource name.
    /// </summary>
    public string Resource { get; private set; }

    /// <summary>
    ///     Gets the action allowed over the resource.
    /// </summary>
    public string Action { get; private set; }

    /// <summary>
    ///     Gets the permission description translation key.
    /// </summary>
    public string Description { get; private set; }
}
