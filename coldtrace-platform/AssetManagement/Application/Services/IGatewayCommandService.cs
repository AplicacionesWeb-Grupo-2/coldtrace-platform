using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Application.Services;

/// <summary>
///     Gateway command application service contract.
/// </summary>
public interface IGatewayCommandService
{
    /// <summary>
    ///     Handles creation of a gateway aggregate.
    /// </summary>
    /// <param name="command">Command containing gateway data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created gateway or a creation error.</returns>
    Task<Result<Gateway, CreateGatewayError>> Handle(
        CreateGatewayCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles update of a gateway aggregate.
    /// </summary>
    /// <param name="command">Command containing updated gateway data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated gateway or an update error.</returns>
    Task<Result<Gateway, UpdateGatewayError>> Handle(
        UpdateGatewayCommand command,
        CancellationToken cancellationToken = default);
}
