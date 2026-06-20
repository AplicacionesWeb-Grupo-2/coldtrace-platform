using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Commands;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Alerts.Domain.Services;

/// <summary>
///     Application service contract for incident commands.
/// </summary>
public interface IIncidentCommandService
{
    Task<Result<Incident, CreateIncidentError>> Handle(
        CreateIncidentCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Incident, AcknowledgeIncidentError>> Handle(
        AcknowledgeIncidentCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Incident, EscalateIncidentError>> Handle(
        EscalateIncidentCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Incident, RegisterIncidentCorrectiveActionError>> Handle(
        RegisterIncidentCorrectiveActionCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<Incident, ResolveIncidentError>> Handle(
        ResolveIncidentCommand command,
        CancellationToken cancellationToken = default);
}
