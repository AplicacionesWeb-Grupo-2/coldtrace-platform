using ColdTrace.Platform.Alerts.Domain.Model.Errors;
using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.Alerts.Domain.Model.Queries;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Application.CommandServices;
using ColdTrace.Platform.Alerts.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Alerts.Application.Internal.QueryServices;

/// <summary>
///     Application service for AI resolution plan query operations.
/// </summary>
public class AiResolutionPlanQueryService(
    IAiResolutionPlanRepository aiResolutionPlanRepository,
    IIncidentRepository incidentRepository,
    IIamContextFacade iamContextFacade,
    ILogger<AiResolutionPlanQueryService> logger)
    : IAiResolutionPlanQueryService
{
    public async Task<Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>> Handle(
        GetAiResolutionPlansByIncidentIdAndOrganizationIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(query.OrganizationId, cancellationToken))
        {
            logger.LogWarning(
                "Organization not found for AI resolution plan history: {OrganizationId}",
                query.OrganizationId);
            return Failure(GetAiResolutionPlansByIncidentError.OrganizationNotFound);
        }

        var incident = await incidentRepository.FindByIdAndOrganizationIdAsync(
            query.IncidentId,
            query.OrganizationId,
            cancellationToken);
        if (incident is null)
        {
            logger.LogWarning(
                "Incident not found for AI resolution plan history: {OrganizationId} {IncidentId}",
                query.OrganizationId,
                query.IncidentId);
            return Failure(GetAiResolutionPlansByIncidentError.IncidentNotFound);
        }

        try
        {
            var plans = await aiResolutionPlanRepository.FindAllByIncidentIdAndOrganizationIdAsync(
                query.IncidentId,
                query.OrganizationId,
                cancellationToken);
            return new Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Success(plans);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error getting AI resolution plan history for incident {IncidentId} in organization {OrganizationId}",
                query.IncidentId,
                query.OrganizationId);
            return Failure(GetAiResolutionPlansByIncidentError.UnexpectedError);
        }
    }

    private static Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError> Failure(
        GetAiResolutionPlansByIncidentError error) =>
        new Result<IEnumerable<AiResolutionPlan>, GetAiResolutionPlansByIncidentError>.Failure(error);
}
