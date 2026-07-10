using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a sign-in command from a request resource.
/// </summary>
public static class SignInCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a sign-in resource into a command.
    /// </summary>
    /// <param name="resource">The sign-in request resource.</param>
    /// <returns>A sign-in command.</returns>
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Email, resource.Password);
}
