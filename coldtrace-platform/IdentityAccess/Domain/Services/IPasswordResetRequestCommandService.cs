using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.IdentityAccess.Domain.Services;

/// <summary>
///     Password reset request command service contract.
/// </summary>
public interface IPasswordResetRequestCommandService
{
    /// <summary>
    ///     Accepts a password reset request without revealing account existence.
    /// </summary>
    Task<Result<PasswordResetRequestResult, CreatePasswordResetRequestError>> Handle(
        CreatePasswordResetRequestCommand command,
        CancellationToken cancellationToken = default);
}
