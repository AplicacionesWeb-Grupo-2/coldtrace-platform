using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;

namespace ColdTrace.Platform.IdentityAccess.Application.Results;

/// <summary>
///     Application result for the organization sign-up use case.
/// </summary>
/// <param name="Organization">Organization created by the sign-up flow.</param>
/// <param name="User">First user created for the organization.</param>
public record OrganizationSignUpResult(Organization Organization, User User);
