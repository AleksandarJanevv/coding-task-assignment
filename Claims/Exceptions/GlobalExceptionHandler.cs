using System.Net;
using Claims.DTO;
using Microsoft.AspNetCore.Diagnostics;

namespace Claims.Exceptions;

/// <summary>
/// Provides centralized exception handling for the Claims API.
/// </summary>
/// <remarks>
/// Implements <see cref="IExceptionHandler"/> to intercept unhandled exceptions
/// and convert them into standardized API error responses.
///
/// Custom API exceptions are translated into responses containing their
/// application-specific return code, details, optional entity identifier,
/// and configured HTTP status code.
///
/// Unexpected exceptions are logged as errors and converted into a generic
/// internal server error response without exposing internal exception details
/// to API consumers.
/// </remarks>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{

    /// <summary>
    /// Attempts to handle an exception by generating a standardized API error response.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception that occurred during request processing.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A value indicating whether the exception was successfully handled.
    /// Returns true after writing the error response.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = HttpStatusCode.InternalServerError;

        ApiErrorResponseDto response;

        if (exception is CustomApiException apiException)
        {
            statusCode = apiException.StatusCode;

            response = new ApiErrorResponseDto
            {
                ReturnCode = apiException.ReturnCode,
                Details = apiException.Details,
                Id = apiException.Id
            };

            logger.LogWarning("API exception: {ReturnCode} - {Details}", apiException.ReturnCode, apiException.Details);
        }
        else
        {
            logger.LogError(exception, "An unexpected error occurred.");

            response = new ApiErrorResponseDto
            {
                ReturnCode = "INTERNAL_ERROR",
                Details = "An unexpected error occurred."
            };
        }

        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}