using System.IdentityModel.Tokens.Jwt;
using System.Text;

using FastEndpoints.Swagger;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

using SourceName.Api.Loaders.Models;

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
        // Register Scalar UI
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

        var jwtBearerTokenSettings = JwtBearerTokenSettings.GetJwtBearerTokenSettings(app.Configuration);
        var key = Encoding.UTF8.GetBytes(jwtBearerTokenSettings.SigningKey);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = signingCredentials,
            Audience = jwtBearerTokenSettings.Audience,
            Issuer = jwtBearerTokenSettings.Issuer,
            Claims = new Dictionary<string, object>
            {
                {JwtRegisteredClaimNames.Sub, "scalar@gardnerwebtech.com" },
                {JwtRegisteredClaimNames.NameId, "f080fbab-5ccc-4f79-aa15-c07959e1b1b5" }
            }
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(token);
    }
}
