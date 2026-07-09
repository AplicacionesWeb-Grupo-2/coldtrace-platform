using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;

/// <summary>
///     Converts password reset request resources to commands.
/// </summary>
public static class CreatePasswordResetRequestCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts the request resource to an application command.
    /// </summary>
    public static CreatePasswordResetRequestCommand ToCommandFromResource(
        CreatePasswordResetRequestResource resource) =>
        new(resource.Email);
}
