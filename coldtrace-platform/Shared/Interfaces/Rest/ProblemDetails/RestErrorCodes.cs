using System.Text;

namespace ColdTrace.Platform.Shared.Interfaces.Rest.ProblemDetails;

/// <summary>
///     Stable machine-readable codes included in RFC 7807 responses.
/// </summary>
public static class RestErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string ResourceNotFound = "RESOURCE_NOT_FOUND";
    public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";
    public const string ResourceConflict = "RESOURCE_CONFLICT";
    public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE";
    public const string BadGateway = "BAD_GATEWAY";
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string GatewayTimeout = "GATEWAY_TIMEOUT";
    public const string UnexpectedError = "UNEXPECTED_ERROR";

    public static string FromResourceKey(string resourceKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        var code = new StringBuilder(resourceKey.Length + 8);
        for (var index = 0; index < resourceKey.Length; index++)
        {
            var current = resourceKey[index];
            if (index > 0 && char.IsUpper(current))
            {
                var previous = resourceKey[index - 1];
                var nextIsLower = index + 1 < resourceKey.Length && char.IsLower(resourceKey[index + 1]);
                if (char.IsLower(previous) || char.IsDigit(previous) || nextIsLower) code.Append('_');
            }

            code.Append(char.ToUpperInvariant(current));
        }

        return code.ToString();
    }

    public static string FromStatusCode(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => InvalidRequest,
            StatusCodes.Status401Unauthorized => Unauthorized,
            StatusCodes.Status403Forbidden => Forbidden,
            StatusCodes.Status404NotFound => ResourceNotFound,
            StatusCodes.Status405MethodNotAllowed => MethodNotAllowed,
            StatusCodes.Status409Conflict => ResourceConflict,
            StatusCodes.Status415UnsupportedMediaType => UnsupportedMediaType,
            StatusCodes.Status502BadGateway => BadGateway,
            StatusCodes.Status503ServiceUnavailable => ServiceUnavailable,
            StatusCodes.Status504GatewayTimeout => GatewayTimeout,
            _ => UnexpectedError
        };
}
