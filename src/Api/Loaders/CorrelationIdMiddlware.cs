using Serilog.Context;

namespace SourceName.Api.Loaders;

public class CorrelationIdMiddlware(RequestDelegate next)
{
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
