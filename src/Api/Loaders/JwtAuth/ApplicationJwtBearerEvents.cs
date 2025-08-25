using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using ILogger = Serilog.ILogger;

namespace SourceName.Api.Loaders.JwtAuth;

/// <inheritdoc />
public class ApplicationJwtBearerEvents(ILogger logger) : JwtBearerEvents
{
    private readonly ILogger _logger = logger;

    /// <inheritdoc />
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        if (context.Exception is SecurityTokenExpiredException)
        {
            context.Response.Headers.Append("Token-Expired", "true");
            return Task.CompletedTask;
        }

        _logger.Error(context.Exception, "JWT Token Error");
        context.Response.Headers.Append("Token-Error", "invalid token");

        return Task.CompletedTask;
    }
}
