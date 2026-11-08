using System.Net;

namespace Claims.Exceptions;

/// <summary>
/// Provides predefined construction methods for creating
/// <see cref="CustomApiException"/> instances.
/// </summary>
/// <remarks>
/// This class acts as a director in the Builder design pattern,
/// encapsulating common exception construction scenarios.
/// It provides methods for creating exceptions with or without
/// an associated entity identifier.
/// </remarks>
public static class CustomExceptionBuilderDirector
{
    /// <summary>
    /// Creates a custom API exception without an associated entity identifier.
    /// </summary>
    /// <param name="returnCode">The application-specific error code.</param>
    /// <param name="details">The human-readable error description.</param>
    /// <param name="statusCode">The application-specific status code.</param>
    /// <returns>A configured <see cref="CustomApiException"/> instance.</returns>
    public static CustomApiException WithoutId(string returnCode, string details, HttpStatusCode statusCode)
    {
        return new CustomApiException.Builder()
            .SetReturnCode(returnCode)
            .SetStatusCode(statusCode)
            .SetDetails(details)
            .Build();
    }

    /// <summary>
    /// Creates a custom API exception associated with a specific entity.
    /// </summary>
    /// <param name="returnCode">The application-specific error code.</param>
    /// <param name="details">The human-readable error description.</param>
    /// <param name="id">The identifier of the entity associated with the error.</param>
    /// <param name="statusCode">The application-specific status code.</param>
    /// <returns>A configured <see cref="CustomApiException"/> instance.</returns>
    public static CustomApiException WithId(string returnCode, string details, string id, HttpStatusCode statusCode)
    {
        return new CustomApiException.Builder()
            .SetReturnCode(returnCode)
            .SetStatusCode(statusCode)
            .SetDetails(details)
            .SetId(id)
            .Build();
    }
}