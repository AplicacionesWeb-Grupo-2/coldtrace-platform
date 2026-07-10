namespace ColdTrace.Platform.Iam.Domain.Model.Queries;

/// <summary>
///     Query for the user associated with an authenticated token.
/// </summary>
/// <param name="UserId">User identifier.</param>
public record GetUserByIdQuery(int UserId);
