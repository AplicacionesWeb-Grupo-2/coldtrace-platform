using System.Text.Json;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;
using ColdTrace.Platform.Billing.Domain.Model.ValueObjects;
using ColdTrace.Platform.Billing.Infrastructure.Configuration;
using ColdTrace.Platform.Shared.Application.Model;
using Microsoft.Extensions.Options;
using Stripe;
using BillingSubscriptionStatuses = ColdTrace.Platform.Billing.Domain.Model.ValueObjects.SubscriptionStatuses;

namespace ColdTrace.Platform.Billing.Infrastructure.Stripe;

/// <summary>
///     Stripe-backed webhook verifier and event normalizer.
/// </summary>
public class StripeBillingWebhookProviderService(
    IOptions<BillingOptions> options,
    ILogger<StripeBillingWebhookProviderService> logger)
    : IBillingWebhookProviderService
{
    private const string Provider = BillingProviders.Stripe;
    private const string EventCheckoutCompleted = "checkout.session.completed";
    private const string EventSubscriptionUpdated = "customer.subscription.updated";
    private const string EventSubscriptionDeleted = "customer.subscription.deleted";
    private const string EventInvoicePaid = "invoice.paid";
    private const string EventInvoicePaymentFailed = "invoice.payment_failed";

    public Task<Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>> ParseSignedEventAsync(
        string payload,
        string? signatureHeader,
        CancellationToken cancellationToken = default)
    {
        var stripeOptions = options.Value.Stripe;
        if (!stripeOptions.HasWebhookConfiguration())
        {
            logger.LogWarning("Stripe webhook configuration is incomplete");
            return Completed(Failure(BillingWebhookProviderFailure.NotConfigured));
        }

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            logger.LogWarning("Stripe webhook rejected because Stripe-Signature is missing");
            return Completed(Failure(BillingWebhookProviderFailure.MissingSignature));
        }

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signatureHeader,
                stripeOptions.WebhookSigningSecret,
                throwOnApiVersionMismatch: false);
            using var document = JsonDocument.Parse(payload);
            var stripeObject = PropertyAt(document.RootElement, "data", "object") ?? default;
            return Completed(
                new Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>.Success(
                    ToProviderEvent(stripeEvent.Id, stripeEvent.Type, stripeObject)));
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe webhook signature verification failed");
            return Completed(Failure(BillingWebhookProviderFailure.InvalidSignature));
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Stripe webhook payload parsing failed");
            return Completed(Failure(BillingWebhookProviderFailure.InvalidPayload));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Stripe webhook payload parsing failed");
            return Completed(Failure(BillingWebhookProviderFailure.InvalidPayload));
        }
    }

    private static BillingWebhookProviderEvent ToProviderEvent(
        string eventId,
        string eventType,
        JsonElement stripeObject) =>
        eventType switch
        {
            EventCheckoutCompleted => CheckoutCompleted(eventId, eventType, stripeObject),
            EventSubscriptionUpdated => SubscriptionUpdated(eventId, eventType, stripeObject),
            EventSubscriptionDeleted => SubscriptionDeleted(eventId, eventType, stripeObject),
            EventInvoicePaid => InvoiceEvent(eventId, eventType, stripeObject, BillingSubscriptionStatuses.Active),
            EventInvoicePaymentFailed => InvoiceEvent(eventId, eventType, stripeObject, BillingSubscriptionStatuses.PastDue),
            _ => Unsupported(eventId, eventType, stripeObject)
        };

    private static BillingWebhookProviderEvent CheckoutCompleted(
        string eventId,
        string eventType,
        JsonElement stripeObject) =>
        new(
            Provider,
            eventId,
            eventType,
            TextAt(stripeObject, "id"),
            OrganizationIdFromCheckoutSession(stripeObject),
            TextOrObjectIdAt(stripeObject, "customer"),
            TextOrObjectIdAt(stripeObject, "subscription"),
            MetadataText(stripeObject, "targetPlanCode"),
            null,
            BillingSubscriptionStatuses.Active,
            null,
            null,
            false,
            true);

    private static BillingWebhookProviderEvent SubscriptionUpdated(
        string eventId,
        string eventType,
        JsonElement stripeObject) =>
        SubscriptionEvent(eventId, eventType, stripeObject, StatusFromStripeSubscription(stripeObject));

    private static BillingWebhookProviderEvent SubscriptionDeleted(
        string eventId,
        string eventType,
        JsonElement stripeObject) =>
        SubscriptionEvent(eventId, eventType, stripeObject, BillingSubscriptionStatuses.Canceled);

    private static BillingWebhookProviderEvent SubscriptionEvent(
        string eventId,
        string eventType,
        JsonElement stripeObject,
        string status) =>
        new(
            Provider,
            eventId,
            eventType,
            TextAt(stripeObject, "id"),
            MetadataInt(stripeObject, "organizationId"),
            TextOrObjectIdAt(stripeObject, "customer"),
            TextAt(stripeObject, "id"),
            MetadataText(stripeObject, "targetPlanCode"),
            SubscriptionPriceId(stripeObject),
            status,
            EpochSecondsAt(stripeObject, "current_period_start"),
            EpochSecondsAt(stripeObject, "current_period_end"),
            BooleanAt(stripeObject, "cancel_at_period_end"),
            true);

    private static BillingWebhookProviderEvent InvoiceEvent(
        string eventId,
        string eventType,
        JsonElement stripeObject,
        string status) =>
        new(
            Provider,
            eventId,
            eventType,
            TextAt(stripeObject, "id"),
            null,
            TextOrObjectIdAt(stripeObject, "customer"),
            InvoiceSubscriptionId(stripeObject),
            null,
            InvoicePriceId(stripeObject),
            status,
            EpochSecondsAt(stripeObject, "period_start"),
            EpochSecondsAt(stripeObject, "period_end"),
            false,
            true);

    private static BillingWebhookProviderEvent Unsupported(
        string eventId,
        string eventType,
        JsonElement stripeObject) =>
        new(
            Provider,
            eventId,
            eventType,
            TextAt(stripeObject, "id"),
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            false);

    private static int? OrganizationIdFromCheckoutSession(JsonElement stripeObject) =>
        MetadataInt(stripeObject, "organizationId") ?? IntFromText(TextAt(stripeObject, "client_reference_id"));

    private static string StatusFromStripeSubscription(JsonElement stripeObject) =>
        TextAt(stripeObject, "status") switch
        {
            "active" or "trialing" => BillingSubscriptionStatuses.Active,
            "canceled" => BillingSubscriptionStatuses.Canceled,
            _ => BillingSubscriptionStatuses.PastDue
        };

    private static string? InvoiceSubscriptionId(JsonElement stripeObject) =>
        TextOrObjectIdAt(stripeObject, "subscription")
        ?? TextAt(stripeObject, "parent", "subscription_details", "subscription");

    private static string? SubscriptionPriceId(JsonElement stripeObject) =>
        TextAt(stripeObject, "items", "data", 0, "price", "id");

    private static string? InvoicePriceId(JsonElement stripeObject) =>
        TextAt(stripeObject, "lines", "data", 0, "price", "id")
        ?? TextAt(stripeObject, "lines", "data", 0, "pricing", "price_details", "price");

    private static string? MetadataText(JsonElement stripeObject, string fieldName) =>
        TextAt(stripeObject, "metadata", fieldName);

    private static int? MetadataInt(JsonElement stripeObject, string fieldName) =>
        IntFromText(MetadataText(stripeObject, fieldName));

    private static DateTimeOffset? EpochSecondsAt(JsonElement stripeObject, string fieldName)
    {
        var value = PropertyAt(stripeObject, fieldName);
        if (value is null || value.Value.ValueKind != JsonValueKind.Number ||
            !value.Value.TryGetInt64(out var epochSeconds))
            return null;

        return DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
    }

    private static bool BooleanAt(JsonElement stripeObject, string fieldName)
    {
        var value = PropertyAt(stripeObject, fieldName);
        return value is { ValueKind: JsonValueKind.True };
    }

    private static string? TextOrObjectIdAt(JsonElement stripeObject, string fieldName)
    {
        var value = PropertyAt(stripeObject, fieldName);
        if (value is null) return null;

        return value.Value.ValueKind switch
        {
            JsonValueKind.String => Normalize(value.Value.GetString()),
            JsonValueKind.Object => TextAt(value.Value, "id"),
            _ => null
        };
    }

    private static string? TextAt(JsonElement stripeObject, params object[] path)
    {
        var value = PropertyAt(stripeObject, path);
        return value is { ValueKind: JsonValueKind.String } ? Normalize(value.Value.GetString()) : null;
    }

    private static JsonElement? PropertyAt(JsonElement stripeObject, params object[] path)
    {
        var current = stripeObject;
        foreach (var segment in path)
        {
            switch (segment)
            {
                case string propertyName:
                    if (current.ValueKind != JsonValueKind.Object ||
                        !current.TryGetProperty(propertyName, out var childProperty))
                        return null;
                    current = childProperty;
                    break;
                case int arrayIndex:
                    if (current.ValueKind != JsonValueKind.Array ||
                        current.GetArrayLength() <= arrayIndex)
                        return null;
                    current = current[arrayIndex];
                    break;
                default:
                    return null;
            }
        }

        return current;
    }

    private static int? IntFromText(string? value)
    {
        if (value is null) return null;
        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>.Failure Failure(
        BillingWebhookProviderFailure failure) => new(failure);

    private static Task<Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure>> Completed(
        Result<BillingWebhookProviderEvent, BillingWebhookProviderFailure> result) =>
        Task.FromResult(result);
}
