using ColdTrace.Platform.Alerts.Application.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;

namespace ColdTrace.Platform.Alerts.Application.Internal.QueryServices;

/// <summary>
///     Application service for incident query operations.
/// </summary>
public class IncidentQueryService(
    IIncidentRepository incidentRepository,
    IOrganizationRepository organizationRepository,
    ILogger<IncidentQueryService> logger)
    : IIncidentQueryService
{
    /// <inheritdoc />
    public async Task<Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>> Handle(
        GetIncidentsByOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for incident query: {OrganizationId}", query.OrganizationId);
            return new Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Failure(
                GetIncidentsByOrganizationError.OrganizationNotFound);
        }

        try
        {
            var incidents = await incidentRepository.FindAllByOrganizationIdAsync(
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Success(incidents);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error getting incidents for organization {OrganizationId}",
                query.OrganizationId);
            return new Result<IEnumerable<Incident>, GetIncidentsByOrganizationError>.Failure(
                GetIncidentsByOrganizationError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Incident, GetIncidentByIdAndOrganizationError>> Handle(
        GetIncidentByIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(query.OrganizationId, cancellationToken);
        if (organization is null)
        {
            logger.LogWarning("Organization not found for incident detail query: {OrganizationId}",
                query.OrganizationId);
            return new Result<Incident, GetIncidentByIdAndOrganizationError>.Failure(
                GetIncidentByIdAndOrganizationError.OrganizationNotFound);
        }

        try
        {
            var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
                query.IncidentId,
                query.OrganizationId,
                cancellationToken);
            if (incident is null)
                return new Result<Incident, GetIncidentByIdAndOrganizationError>.Failure(
                    GetIncidentByIdAndOrganizationError.IncidentNotFound);

            return new Result<Incident, GetIncidentByIdAndOrganizationError>.Success(incident);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error getting incident {IncidentId} for organization {OrganizationId}",
                query.IncidentId,
                query.OrganizationId);
            return new Result<Incident, GetIncidentByIdAndOrganizationError>.Failure(
                GetIncidentByIdAndOrganizationError.UnexpectedError);
        }
    }
}
