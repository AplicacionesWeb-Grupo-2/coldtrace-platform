using ColdTrace.Platform.Iam.Domain.Model.Aggregates;

namespace ColdTrace.Platform.Iam.Application.Results;

/// <summary>
///     Application result returned after successful authentication.
/// </summary>
/// <param name="User">Authenticated user.</param>
/// <param name="Token">Issued JWT bearer token.</param>
public record AuthenticatedUserResult(User User, string Token);
