using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Interfaces.Rest.Resources;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace ColdTrace.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Converts social authentication results to the Open Source-compatible REST contract.
/// </summary>
public static class ActionResultFromSocialAuthenticationResultAssembler
{
    public static ActionResult ToAuthenticatedUserActionResult(
        Result<AuthenticatedUserResult, SocialAuthenticationError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<AuthenticatedUserResult, SocialAuthenticationError>.Success success =>
                controller.Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                    success.Value.User,
                    success.Value.Token)),
            Result<AuthenticatedUserResult, SocialAuthenticationError>.Failure failure =>
                ToErrorActionResult(failure.Error, controller),
            _ => ToErrorActionResult(
                SocialAuthenticationError.Unexpected("identity-access.authentication.error.unexpected"),
                controller)
        };

    public static ActionResult ToProfileActionResult(
        Result<SocialIdentityProfileResult, SocialAuthenticationError> result,
        ControllerBase controller) =>
        result switch
        {
            Result<SocialIdentityProfileResult, SocialAuthenticationError>.Success success =>
                controller.Ok(SocialIdentityProfileResourceFromResultAssembler.ToResourceFromResult(success.Value)),
            Result<SocialIdentityProfileResult, SocialAuthenticationError>.Failure failure =>
                ToErrorActionResult(failure.Error, controller),
            _ => ToErrorActionResult(
                SocialAuthenticationError.Unexpected("identity-access.authentication.error.unexpected"),
                controller)
        };

    private static ActionResult ToErrorActionResult(
        SocialAuthenticationError error,
        ControllerBase controller)
    {
        var statusCode = error.Code switch
        {
            "PROVIDER_VALIDATION_FAILED" => StatusCodes.Status401Unauthorized,
            "SOCIAL_IDENTITY_REQUIRES_ONBOARDING" => StatusCodes.Status422UnprocessableEntity,
            "SOCIAL_PROVIDER_CONFIGURATION_MISSING" => StatusCodes.Status503ServiceUnavailable,
            "UNEXPECTED_ERROR" => StatusCodes.Status500InternalServerError,
            var code when code.EndsWith("_NOT_FOUND", StringComparison.Ordinal) => StatusCodes.Status404NotFound,
            var code when code.EndsWith("_CONFLICT", StringComparison.Ordinal) => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return controller.StatusCode(
            statusCode,
            new AuthenticationErrorResource(error.Code, error.Message, error.Details));
    }
}
