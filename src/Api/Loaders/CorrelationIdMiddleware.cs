using Serilog.Context;

namespace SourceName.Api.Loaders;

/// <summary>
/// Adds CorrelationId to the logging context
/// </summary>
/// <param name="next"><see cref="RequestDelegate"/></param>
public class CorrelationIdMiddleware(RequestDelegate next)
{
    /// <summary>
    /// Adds CorrelationId to the logging context
    /// </summary>
    /// <param name="context"><see cref="HttpContext"/></param>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("Correlation-Id", out var correlationIds);

        var correlationId = correlationIds.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = context.TraceIdentifier;
            context.Items.Add("Correlation-Id", correlationId);
            context.Response.Headers.Append("Correlation-Id", correlationId);
        }

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
