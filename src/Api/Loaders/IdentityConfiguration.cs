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
        services.AddFastEndpointsAuthentication();
        services.AddAuthorization();
        
        return services;
    }

    private static void AddFastEndpointsAuthentication(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var tokenService = serviceProvider.GetRequiredService<JwtTokenService>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
        services.AddAuthenticationJwtBearer(
            s => s.SigningKey = tokenService.GetSigningKey(),
            b =>
            {
                b.TokenValidationParameters = tokenService.GetTokenValidationParameters();
                b.Events = serviceProvider.GetRequiredService<ApplicationJwtBearerEvents>();
            });
    }
}
