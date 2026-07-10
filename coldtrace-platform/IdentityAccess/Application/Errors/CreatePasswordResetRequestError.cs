namespace ColdTrace.Platform.IdentityAccess.Application.Errors;

/// <summary>
///     Errors that can occur while preparing a password reset request.
/// </summary>
public enum CreatePasswordResetRequestError
{
    /// <summary>
    ///     The reset request metadata could not be prepared or persisted.
    /// </summary>
    UnexpectedError
}
