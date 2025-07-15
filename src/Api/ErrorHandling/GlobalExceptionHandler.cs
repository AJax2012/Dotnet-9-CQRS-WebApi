using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using ILogger = Serilog.ILogger;

namespace SourceName.Api.ErrorHandling;

internal sealed class GlobalExceptionHandler(ILogger logger, IWebHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.Error(exception, "Unhandled exception occured");
        
        var problemDetails = new ProblemDetails
        {
            Title = "An error occured",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc9110#name-500-internal-server-error",
            Extensions =
            {
                { "traceId", httpContext.TraceIdentifier }
            }
        };
        
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
