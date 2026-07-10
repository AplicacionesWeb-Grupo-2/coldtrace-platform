namespace ColdTrace.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks a controller or action as publicly accessible.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute;
