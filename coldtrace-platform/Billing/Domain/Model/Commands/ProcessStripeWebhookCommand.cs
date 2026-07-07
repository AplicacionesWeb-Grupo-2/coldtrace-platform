namespace ColdTrace.Platform.Billing.Domain.Model.Commands;

/// <summary>
///     Command for processing one signed Stripe webhook request.
/// </summary>
public record ProcessStripeWebhookCommand
{
    public ProcessStripeWebhookCommand(string payload, string? signatureHeader)
    {
        Payload = payload ?? string.Empty;
        SignatureHeader = string.IsNullOrWhiteSpace(signatureHeader) ? null : signatureHeader.Trim();
    }

    public string Payload { get; init; }

    public string? SignatureHeader { get; init; }
}
