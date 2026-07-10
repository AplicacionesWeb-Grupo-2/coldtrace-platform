using System.Buffers.Text;
using System.Security.Cryptography;
using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;

/// <summary>
///     Application service for password reset requests.
/// </summary>
/// <param name="userRepository">User repository.</param>
/// <param name="passwordResetRequestRepository">Password reset request repository.</param>
/// <param name="unitOfWork">Unit of work for persistence.</param>
/// <param name="logger">Logger for diagnostics.</param>
public class PasswordResetRequestCommandService(
    IUserRepository userRepository,
    IPasswordResetRequestRepository passwordResetRequestRepository,
    IUnitOfWork unitOfWork,
    ILogger<PasswordResetRequestCommandService> logger)
    : IPasswordResetRequestCommandService
{
    private const int TokenByteLength = 32;
    private const string DeliveryNotConfigured = "EMAIL_DELIVERY_NOT_CONFIGURED";
    private static readonly TimeSpan TokenTimeToLive = TimeSpan.FromMinutes(30);

    /// <inheritdoc />
    public async Task<Result<PasswordResetRequestResult, CreatePasswordResetRequestError>> Handle(
        CreatePasswordResetRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        var requestedAt = DateTimeOffset.UtcNow;
        var expiresAt = requestedAt.Add(TokenTimeToLive);
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("Password reset requested for a non-existing email");
            return Accepted(requestedAt, expiresAt);
        }

        try
        {
            var rawToken = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenByteLength));
            var tokenHash = Convert.ToHexStringLower(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
            var request = new PasswordResetRequest(
                command.Email,
                user.Id,
                tokenHash,
                requestedAt,
                expiresAt);

            await passwordResetRequestRepository.AddAsync(request, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            logger.LogInformation("Password reset request accepted for user {UserId}", user.Id);
            return Accepted(requestedAt, expiresAt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Password reset request could not be prepared for user {UserId}", user.Id);
            return new Result<PasswordResetRequestResult, CreatePasswordResetRequestError>.Failure(
                CreatePasswordResetRequestError.UnexpectedError);
        }
    }

    private static Result<PasswordResetRequestResult, CreatePasswordResetRequestError> Accepted(
        DateTimeOffset requestedAt,
        DateTimeOffset expiresAt) =>
        new Result<PasswordResetRequestResult, CreatePasswordResetRequestError>.Success(
            new PasswordResetRequestResult(true, requestedAt, expiresAt, DeliveryNotConfigured));
}
