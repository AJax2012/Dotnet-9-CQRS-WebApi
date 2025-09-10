using FastEndpoints;

using ProblemDetails = Microsoft.AspNetCore.Mvc.ProblemDetails;

namespace SourceName.Api.ErrorHandling;

/// <summary>
/// When an unhandled exception occurs, this processor will be called to handle it and return a problem details response.
/// </summary>
internal sealed class GlobalExceptionProcessor(ILoggerFactory logger)
    : IGlobalPostProcessor
{
    private readonly ILogger _logger = logger.CreateLogger<GlobalExceptionProcessor>();

    public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken cancellationToken)
    {
        if (!context.HasExceptionOccurred)
        {
            return;
        }

        var exception = context.ExceptionDispatchInfo!.SourceException;
        var httpContext = context.HttpContext;

        _logger.LogError(exception, "An unhandled exception occured");

        var problemDetails = new ProblemDetails
        {
            Title = "An error occured",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc9110#name-500-internal-server-error"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        context.MarkExceptionAsHandled();
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
