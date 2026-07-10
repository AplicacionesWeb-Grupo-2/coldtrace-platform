using ColdTrace.Platform.AssetManagement.Application.Errors;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.AssetManagement.Domain.Services;

/// <summary>
///     Location command application service contract.
/// </summary>
public interface ILocationCommandService
{
    /// <summary>
    ///     Handles creation of a location aggregate.
    /// </summary>
    /// <param name="command">Command containing location data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The created location or a creation error.</returns>
    Task<Result<Location, CreateLocationError>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles update of a location aggregate.
    /// </summary>
    /// <param name="command">Command containing updated location data.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The updated location or an update error.</returns>
    Task<Result<Location, UpdateLocationError>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Handles deletion of an organization-scoped location.
    /// </summary>
    /// <param name="command">Command containing route-scoped deletion identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The handled command or a deletion error.</returns>
    Task<Result<DeleteLocationCommand, DeleteLocationError>> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken = default);
}
