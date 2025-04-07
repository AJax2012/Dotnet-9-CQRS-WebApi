using FastEndpoints.Swagger;
using Scalar.AspNetCore;

namespace SourceName.Api.Loaders;

internal static class OpenApiConfiguration
{
    internal static IServiceCollection AddOpenApiDocuments(this IServiceCollection services)
    {
        return services
            .AddOpenApi()
            .SwaggerDocument(opts =>
            {
                opts.ShortSchemaNames = true;
                opts.AutoTagPathSegmentIndex = 0;
                opts.ExcludeNonFastEndpoints = true;
            });
    }

    internal static void UseOpenApiUi(this WebApplication app)
    {
        // Register Swagger UI
        app.MapOpenApi();
        app.UseSwaggerGen();
        
        // Register Scalar UI
        app.UseOpenApi(c => c.Path = "/openapi/scalar/{documentName}.json");
        app.MapScalarApiReference(opts =>
            opts.WithTitle("SourceName API Reference")
                .WithOpenApiRoutePattern("/openapi/scalar/{documentName}.json")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .WithHttpBearerAuthentication(bearerOptions => bearerOptions.Token = "Bearer"));
    }
}
