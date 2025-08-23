using FastEndpoints;
using Serilog;
using SourceName.Api.ErrorHandling;

namespace SourceName.Api.Loaders;

internal static class ApiModule
{
    internal static IServiceCollection AddApiModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddIdentityConfiguration(configuration)
            .AddCorsConfiguration(configuration)
            .AddFastEndpoints()
            .AddProblemDetails()
            .AddOpenApiDocuments();
        
        return services;
    }
    
    internal static void UseApiModule(this WebApplication app, bool isDevelopment)
    {
        app
            .UseAuthentication()
            .UseAuthorization()
            .UseCorsConfiguration(isDevelopment)
            .UseSerilogRequestLogging()
            .UseHttpsRedirection();
        
        app.MapDefaultEndpoints()
            .UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Endpoints.Configurator = cfg =>
                {
                    cfg.DontThrowIfValidationFails();
                    cfg.DontCatchExceptions();
                    cfg.PostProcessor<GlobalExceptionProcessor>(Order.After);
                };
            });
    }
}
