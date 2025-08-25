using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using FastEndpoints.Swagger;

using Scalar.AspNetCore;

using SourceName.Api.Loaders.JwtAuth;

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

    internal static void UseScalar(this WebApplication app)
    {
        app.UseOpenApi(c => c.Path = "/openapi/scalar/{documentName}.json");
        app.MapScalarApiReference(opts =>
            opts.WithTitle("SourceName API Reference")
                .WithOpenApiRoutePattern("/openapi/scalar/{documentName}.json")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
                .AddPreferredSecuritySchemes("JWTBearerAuth")
                .AddHttpAuthentication("JWTBearerAuth", auth =>
                {
                    auth.Token = GenerateSampleJwtToken(app);
                }));
    }

    private static string? GenerateSampleJwtToken(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return null;
        }

        var jwtBearerConfiguration = app.Services.GetRequiredService<JwtTokenService>();

        var identity = new ClaimsIdentity(
        [
            new(JwtRegisteredClaimNames.Sub, "scalar@gardnerwebtech.com"),
            new(JwtRegisteredClaimNames.NameId, Guid.NewGuid().ToString())
        ]);

        return jwtBearerConfiguration.GenerateJwtToken(identity);
    }
}
