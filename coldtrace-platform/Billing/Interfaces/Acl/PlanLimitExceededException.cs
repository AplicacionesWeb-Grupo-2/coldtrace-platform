namespace ColdTrace.Platform.Billing.Interfaces.Acl;

/// <summary>
///     Raised when an organization subscription does not allow a restricted operation.
/// </summary>
public sealed class PlanLimitExceededException(
    string messageResourceKey,
    EntitlementCheckSnapshot entitlement)
    : InvalidOperationException(messageResourceKey)
{
    public string MessageResourceKey { get; } = messageResourceKey;

    public EntitlementCheckSnapshot Entitlement { get; } = entitlement;
}
