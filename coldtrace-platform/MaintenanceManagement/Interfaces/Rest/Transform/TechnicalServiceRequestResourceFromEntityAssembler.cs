using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Resources;

namespace ColdTrace.Platform.MaintenanceManagement.Interfaces.Rest.Transform;

/// <summary>
///     Assembles a technical service request REST resource from a domain entity.
/// </summary>
public static class TechnicalServiceRequestResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="TechnicalServiceRequest" /> into a <see cref="TechnicalServiceRequestResource" />.
    /// </summary>
    public static TechnicalServiceRequestResource ToResourceFromEntity(TechnicalServiceRequest request) =>
        new(request.Id,
            request.OrganizationId,
            request.Code,
            request.AssetId,
            request.AssetLocationId,
            request.AssetName,
            request.IncidentId,
            request.IssueDescription,
            request.Priority,
            request.Status,
            request.RequestedBy,
            request.RequestedAt,
            request.ClosedAt,
            request.ClosureSummary,
            request.Evidence,
            request.ClosedBy,
            request.CreatedAt,
            request.UpdatedAt);
}
