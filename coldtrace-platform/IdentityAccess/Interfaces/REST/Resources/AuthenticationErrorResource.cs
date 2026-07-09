using Swashbuckle.AspNetCore.Annotations;

namespace ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;

/// <summary>
///     Response resource returned when authentication cannot be completed.
/// </summary>
[SwaggerSchema(Description = "Authentication error response")]
public record AuthenticationErrorResource(
    [SwaggerParameter(Description = "Stable application error code")]
    string Code,
    [SwaggerParameter(Description = "Human-readable error message")]
    string Message,
    [SwaggerParameter(Description = "Bounded-context error detail")]
    string Details);
