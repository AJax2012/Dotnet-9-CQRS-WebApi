using System.Text;

using FastEndpoints.Security;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using SourceName.Api.Loaders.Models;

using ILogger = Serilog.ILogger;

namespace SourceName.Api.Loaders;

internal static class IdentityConfiguration
{
    internal static WebApplicationBuilder AddIdentityConfiguration(
        this WebApplicationBuilder app,
        ILogger logger)
    {
        var jwtSettings = app.Configuration.GetJwtBearerTokenSettings();
        var isDevelopment = app.Environment.IsDevelopment();
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
                
                bearerOptions.Events = new()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        else
                        {
                            logger.Error("JWT Token Error: {Message}", context.Exception.Message);
                            context.Response.Headers.Append("Token-Error", "invalid token");
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = _ => Task.CompletedTask,
                };
            });
        
        app.Services.AddAuthorization();
        return app;
    }
    
    private static JwtBearerTokenSettings GetJwtBearerTokenSettings(this IConfiguration configuration)
    {
        var jwtSettings = new JwtBearerTokenSettings();
        
        configuration.GetRequiredSection(JwtBearerTokenSettings.Key)
            .Bind(jwtSettings);
        
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtSettings.Audience);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtSettings.Issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtSettings.SigningKey);
        ArgumentNullException.ThrowIfNull(jwtSettings.ExpiryTimeInSeconds);
        
        return jwtSettings;
    }
}
