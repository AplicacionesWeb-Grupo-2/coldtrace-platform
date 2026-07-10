using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.Billing.Interfaces.Acl;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Errors;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Commands;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.CommandServices;
using ColdTrace.Platform.MaintenanceManagement.Application.QueryServices;
using ColdTrace.Platform.Shared.Application.Model;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.MaintenanceManagement.Application.Internal.CommandServices;

/// <summary>
///     Application service for technical service request command operations.
/// </summary>
public class TechnicalServiceRequestCommandService(
    ITechnicalServiceRequestRepository technicalServiceRequestRepository,
    IIamContextFacade iamContextFacade,
    IAssetRepository assetRepository,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<TechnicalServiceRequestCommandService> logger)
    : ITechnicalServiceRequestCommandService
{
    /// <inheritdoc />
    public async Task<Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>> Handle(
        CreateTechnicalServiceRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning("Organization not found for technical service request creation: {OrganizationId}",
                command.OrganizationId);
            return new Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Failure(
                CreateTechnicalServiceRequestError.OrganizationNotFound);
        }

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
            command.AssetId, command.OrganizationId, cancellationToken);
        if (asset is null)
        {
            logger.LogWarning(
                "Asset not found for technical service request creation: org={OrganizationId} asset={AssetId}",
                command.OrganizationId, command.AssetId);
            return new Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Failure(
                CreateTechnicalServiceRequestError.AssetNotFound);
        }

        await subscriptionBillingContextFacade.EnsureEntitlementAsync(
            command.OrganizationId,
            ISubscriptionBillingContextFacade.EntitlementMaintenance,
            "TechnicalServiceRequestPlanLimitExceeded",
            cancellationToken);

        try
        {
            var request = new TechnicalServiceRequest(command, asset);
            await technicalServiceRequestRepository.AddAsync(request, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation(
                "Technical service request created: {Id} code={Code} org={OrganizationId} asset={AssetId}",
                request.Id, request.Code, request.OrganizationId, request.AssetId);
            return new Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Success(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating technical service request for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<TechnicalServiceRequest, CreateTechnicalServiceRequestError>.Failure(
                CreateTechnicalServiceRequestError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>> Handle(
        UpdateTechnicalServiceRequestStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
        {
            logger.LogWarning(
                "Organization not found for technical service request status update: {OrganizationId}",
                command.OrganizationId);
            return new Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Failure(
                UpdateTechnicalServiceRequestStatusError.OrganizationNotFound);
        }

        var request = await technicalServiceRequestRepository.FindByIdAndOrganizationIdAsync(
            command.TechnicalServiceRequestId, command.OrganizationId, cancellationToken);
        if (request is null)
        {
            logger.LogWarning(
                "Technical service request not found for status update: org={OrganizationId} id={Id}",
                command.OrganizationId, command.TechnicalServiceRequestId);
            return new Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Failure(
                UpdateTechnicalServiceRequestStatusError.TechnicalServiceRequestNotFound);
        }

        if (!request.CanTransitionTo(command.Status))
        {
            logger.LogWarning(
                "Invalid status transition for technical service request {Id}: {From} -> {To}",
                request.Id, request.Status, command.Status);
            return new Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Failure(
                UpdateTechnicalServiceRequestStatusError.InvalidStatusTransition);
        }

        try
        {
            request.UpdateStatus(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Technical service request {Id} status updated to {Status}", request.Id,
                request.Status);
            return new Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Success(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating technical service request {Id} status",
                command.TechnicalServiceRequestId);
            return new Result<TechnicalServiceRequest, UpdateTechnicalServiceRequestStatusError>.Failure(
                UpdateTechnicalServiceRequestStatusError.UnexpectedError);
        }
    }
}
