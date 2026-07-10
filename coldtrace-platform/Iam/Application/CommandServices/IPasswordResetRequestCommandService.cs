using ColdTrace.Platform.Iam.Domain.Model.Errors;
using ColdTrace.Platform.Iam.Application.Results;
using ColdTrace.Platform.Iam.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Iam.Application.CommandServices;

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
