using FastEndpoints;

using SourceName.Api.ErrorHandling;

namespace SourceName.Api.Loaders;

internal static class ApiModule
{
    internal static IServiceCollection AddApiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .AddCorsConfiguration(configuration)
            .AddFastEndpoints()
            .AddOpenApiDocuments();
        
        return services;
    }
}
