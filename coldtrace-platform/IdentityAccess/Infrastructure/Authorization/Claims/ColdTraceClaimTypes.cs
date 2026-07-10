namespace ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Claims;

/// <summary>
///     Claim names carried by ColdTrace access tokens.
/// </summary>
public static class ColdTraceClaimTypes
{
    /// <summary>
    ///     Identifier of the organization that owns the authenticated user.
    /// </summary>
    public const string OrganizationId = "organizationId";

    /// <summary>
    ///     Identifier of the authenticated user's assigned role.
    /// </summary>
    public const string RoleId = "roleId";
}
