using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SourceName.Api.Loaders.JwtAuth;

/// <inheritdoc />
public class ApplicationJwtBearerEvents(ILoggerFactory logger) : JwtBearerEvents
{
    private readonly ILogger _logger = logger.CreateLogger<ApplicationJwtBearerEvents>();

    /// <inheritdoc />
    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        if (context.Exception is SecurityTokenExpiredException)
        {
            context.Response.Headers.Append("Token-Expired", "true");
            return Task.CompletedTask;
        }

        _logger.LogError(context.Exception, "JWT Token Error");
        context.Response.Headers.Append("Token-Error", "invalid token");

        return Task.CompletedTask;
    }
}
