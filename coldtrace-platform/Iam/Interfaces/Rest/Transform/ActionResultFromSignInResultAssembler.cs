using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembles an HTTP action result from a sign-in result.
/// </summary>
public static class ActionResultFromSignInResultAssembler
{
    private const string InvalidCredentialsCode = "INVALID_CREDENTIALS";
    private const string InvalidCredentialsMessage = "Invalid email or password";
    private const string InvalidCredentialsDetails = "identity-access.authentication.error.invalid-credentials";

    /// <summary>
    ///     Converts a sign-in result into an HTTP action result.
    /// </summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller used to produce action results.</param>
    /// <returns>An HTTP action result.</returns>
    public static ActionResult ToActionResultFromSignInResult(
        Result<AuthenticatedUserResult, AuthenticationError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<AuthenticatedUserResult, AuthenticationError>.Success success =>
                controller.Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                    success.Value.User,
                    success.Value.Token)),

            Result<AuthenticatedUserResult, AuthenticationError>.Failure
            { Error: AuthenticationError.InvalidCredentials } =>
                controller.Unauthorized(new AuthenticationErrorResource(
                    InvalidCredentialsCode,
                    InvalidCredentialsMessage,
                    InvalidCredentialsDetails)),

            _ => controller.Problem(
                title: "Unexpected server error",
                detail: "An unexpected error occurred while processing the authentication request.",
                statusCode: StatusCodes.Status500InternalServerError)
        };
}
