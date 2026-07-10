using ColdTrace.Platform.Billing.Domain.Model.Errors;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;
using ColdTrace.Platform.Billing.Application.Results;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Application.CommandServices;
using ColdTrace.Platform.Billing.Application.QueryServices;
using ColdTrace.Platform.Iam.Interfaces.Acl;
using ColdTrace.Platform.Shared.Application.Model;

namespace ColdTrace.Platform.Billing.Application.Internal.CommandServices;

/// <summary>
///     Application service implementation for provider-hosted customer portal session creation.
/// </summary>
public class BillingPortalSessionCommandService(
    IIamContextFacade iamContextFacade,
    IOrganizationSubscriptionRepository organizationSubscriptionRepository,
    IPortalSessionProviderService portalSessionProviderService,
    ILogger<BillingPortalSessionCommandService> logger)
    : IBillingPortalSessionCommandService
{
    public async Task<Result<BillingPortalSession, BillingPortalSessionError>> Handle(
        CreateBillingPortalSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await iamContextFacade.OrganizationExistsAsync(command.OrganizationId, cancellationToken))
            {
                logger.LogWarning("Organization not found for portal session: {OrganizationId}",
                    command.OrganizationId);
                return Failure(BillingPortalSessionError.OrganizationNotFound);
            }

            var subscription = await organizationSubscriptionRepository.FindByOrganizationIdAsync(
                command.OrganizationId,
                cancellationToken);
            if (subscription is null)
            {
                logger.LogWarning("Organization subscription not found for portal session: {OrganizationId}",
                    command.OrganizationId);
                return Failure(BillingPortalSessionError.OrganizationSubscriptionNotFound);
            }

            if (string.IsNullOrWhiteSpace(subscription.ProviderCustomerId))
            {
                logger.LogWarning("Organization has no provider customer id for portal session: {OrganizationId}",
                    command.OrganizationId);
                return Failure(BillingPortalSessionError.ProviderCustomerNotFound);
            }

            var providerResult = await portalSessionProviderService.CreateCustomerPortalSessionAsync(
                new PortalSessionProviderRequest(command.OrganizationId, subscription.ProviderCustomerId),
                cancellationToken);

            return providerResult switch
            {
                Result<PortalSessionProviderResult, PortalSessionProviderFailure>.Success success =>
                    new Result<BillingPortalSession, BillingPortalSessionError>.Success(
                        new BillingPortalSession(
                            success.Value.Provider,
                            success.Value.SessionId,
                            success.Value.PortalUrl,
                            command.OrganizationId)),
                Result<PortalSessionProviderResult, PortalSessionProviderFailure>.Failure failure =>
                    Failure(ToCommandError(failure.Error)),
                _ => Failure(BillingPortalSessionError.ProviderUnavailable)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating portal session for organization {OrganizationId}",
                command.OrganizationId);
            return Failure(BillingPortalSessionError.UnexpectedError);
        }
    }

    private static Result<BillingPortalSession, BillingPortalSessionError>.Failure Failure(
        BillingPortalSessionError error) => new(error);

    private static BillingPortalSessionError ToCommandError(PortalSessionProviderFailure failure) =>
        failure == PortalSessionProviderFailure.NotConfigured
            ? BillingPortalSessionError.ProviderNotConfigured
            : BillingPortalSessionError.ProviderUnavailable;
}
