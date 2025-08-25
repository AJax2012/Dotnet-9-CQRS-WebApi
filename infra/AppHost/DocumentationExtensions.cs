using System.Diagnostics;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SourceName.AppHost;

internal static class DocumentationExtensions
{
    internal static IResourceBuilder<T> WithSwaggerUiDocs<T>(this IResourceBuilder<T> resourceBuilder)
        where T : IResourceWithEndpoints
        => WithOpenApiDocuments(resourceBuilder, "swagger-ui-docs", "Swagger UI Documentation", "swagger");

    internal static IResourceBuilder<T> WithScalarUiDocs<T>(this IResourceBuilder<T> resourceBuilder)
        where T : IResourceWithEndpoints
        => WithOpenApiDocuments(resourceBuilder, "scalar-ui-docs", "Scalar UI Documentation", "scalar/v1");

    private static IResourceBuilder<T> WithOpenApiDocuments<T>(
        this IResourceBuilder<T> resourceBuilder,
        string name,
        string displayName,
        string openApiUiPath)
        where T : IResourceWithEndpoints
    {
        return resourceBuilder.WithCommand(
            name,
            displayName,
            executeCommand: _ =>
            {
                var baseUrl = resourceBuilder.GetEndpoint("https");
                var url = $"{baseUrl.Url}/{openApiUiPath}";

                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return Task.FromResult(new ExecuteCommandResult { Success = true });
            },
            new()
            {
                IconName = "Document",
                UpdateState = context =>
                    context.ResourceSnapshot.HealthStatus == HealthStatus.Healthy ?
                        ResourceCommandState.Enabled :
                        ResourceCommandState.Disabled
            });
    }
}
