using Microsoft.AspNetCore.Authentication.JwtBearer;
using FastEndpoints.Security;
using SourceName.Api.Loaders.JwtAuth;

namespace SourceName.Api.Loaders;

internal static class IdentityConfiguration
{
    internal static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<BearerAuthorizationScheme>()
            .Bind(configuration.GetSection(BearerAuthorizationScheme.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton<JwtTokenService>();
        services.AddTransient<ApplicationJwtBearerEvents>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddFastEndpointsAuthentication();
        services.AddAuthorization();
        
        return services;
    }

    private static void AddFastEndpointsAuthentication(this IServiceCollection services)
    {
        var tokenService = services
            .BuildServiceProvider()
            .GetRequiredService<JwtTokenService>();
        
        services.AddAuthenticationJwtBearer(
            s => s.SigningKey = tokenService.GetSigningKey(),
            b =>
            {
                b.TokenValidationParameters = tokenService.GetTokenValidationParameters();
                b.EventsType = typeof(ApplicationJwtBearerEvents);
            });
    }
}
