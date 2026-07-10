namespace ColdTrace.Platform.Iam.Domain.Model.Errors;

/// <summary>
///     Errors that can occur while authenticating a user.
/// </summary>
public enum AuthenticationError
{
    /// <summary>
    ///     The email and password do not identify a user with valid credentials.
    /// </summary>
    InvalidCredentials
}
