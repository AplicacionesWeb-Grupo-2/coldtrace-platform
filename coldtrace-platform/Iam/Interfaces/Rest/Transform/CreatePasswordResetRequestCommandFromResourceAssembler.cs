using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

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
