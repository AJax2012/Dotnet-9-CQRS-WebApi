using System.Text;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using SourceName.Api.Loaders.Events;
using SourceName.Api.Loaders.Models;
using ILogger = Serilog.ILogger;

namespace SourceName.Api.Loaders;

internal static class IdentityConfiguration
{
    internal static WebApplicationBuilder AddIdentityConfiguration(
        this WebApplicationBuilder app,
        ILogger logger)
    {
        var jwtSettings = JwtBearerTokenSettings.GetJwtBearerTokenSettings(app.Configuration);
        
        app.Services.AddAuthenticationJwtBearer(
            signingOptions => signingOptions.SigningKey = jwtSettings.SigningKey,
            bearerOptions =>
            {
                bearerOptions.TokenValidationParameters = new()
                {
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                };
                
                bearerOptions.Events = new ApplicationJwtBearerEvents(logger);
            });
        
        app.Services.AddAuthorization();
        return app;
    }
}
