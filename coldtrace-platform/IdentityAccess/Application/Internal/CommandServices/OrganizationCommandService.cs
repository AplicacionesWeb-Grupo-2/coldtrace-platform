using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;

/// <summary>
///     Application service for organization command operations.
/// </summary>
/// <param name="organizationRepository">Organization repository.</param>
/// <param name="unitOfWork">Unit of work for persistence.</param>
/// <param name="logger">Logger for diagnostics.</param>
public class OrganizationCommandService(
    IOrganizationRepository organizationRepository,
    ISubscriptionBillingContextFacade subscriptionBillingContextFacade,
    IUnitOfWork unitOfWork,
    ILogger<OrganizationCommandService> logger)
    : IOrganizationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Organization, CreateOrganizationError>> Handle(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await organizationRepository.ExistsByContactEmailAsync(command.ContactEmail, cancellationToken))
        {
            logger.LogWarning("Duplicate organization contact email rejected: {ContactEmail}", command.ContactEmail);
            return new Result<Organization, CreateOrganizationError>.Failure(
                CreateOrganizationError.DuplicateContactEmail);
        }

        if (!string.IsNullOrWhiteSpace(command.TaxId) &&
            await organizationRepository.ExistsByTaxIdAsync(command.TaxId, cancellationToken))
        {
            logger.LogWarning("Duplicate organization tax id rejected: {TaxId}", command.TaxId);
            return new Result<Organization, CreateOrganizationError>.Failure(CreateOrganizationError.DuplicateTaxId);
        }

        try
        {
            var organization = new Organization(command);
            await organizationRepository.AddAsync(organization, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            await subscriptionBillingContextFacade.InitializeBaseSubscriptionForOrganizationAsync(
                organization.Id,
                cancellationToken);
            return new Result<Organization, CreateOrganizationError>.Success(organization);
        }
        catch (DbUpdateException ex)
        {
            if (TryGetDuplicateKeyError(ex, out var duplicateError))
            {
                logger.LogWarning(ex, "Duplicate key violation creating organization with contact email {ContactEmail}",
                    command.ContactEmail);
                return new Result<Organization, CreateOrganizationError>.Failure(duplicateError);
            }

            logger.LogError(ex, "Database update failed creating organization with contact email {ContactEmail}",
                command.ContactEmail);
            return new Result<Organization, CreateOrganizationError>.Failure(CreateOrganizationError.UnexpectedError);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating organization with contact email {ContactEmail}",
                command.ContactEmail);
            return new Result<Organization, CreateOrganizationError>.Failure(CreateOrganizationError.UnexpectedError);
        }
    }

    private static bool TryGetDuplicateKeyError(
        DbUpdateException exception,
        out CreateOrganizationError duplicateError)
    {
        duplicateError = CreateOrganizationError.DuplicateContactEmail;

        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int) &&
                numberProperty.GetValue(current) is int errorCode &&
                errorCode == 1062)
            {
                if (current.Message.Contains("i_x_organizations_tax_id", StringComparison.OrdinalIgnoreCase))
                    duplicateError = CreateOrganizationError.DuplicateTaxId;

                return true;
            }
        }

        return false;
    }
}
