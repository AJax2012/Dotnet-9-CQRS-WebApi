namespace SourceName.Api.Loaders;

internal static class CorsConfiguration
{
    private const string AllowClientOrigin = "SourceNameOrigin";
    private const string AllowScalarOrigin = "ScalarOrigin";

    internal static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var clientOrigin = configuration.GetSection("Authentication:ClientOrigin").Value;
        var env = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT")!;

        services.AddCors(options =>
        {
            options.AddPolicy(
                name: AllowClientOrigin,
                policy =>
                {
                    policy
                        .WithOrigins(clientOrigin!)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

            if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                options.AddPolicy(AllowScalarOrigin, policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            }
        });

        return services;
    }

    internal static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app, bool isDevelopment)
    {
        app.UseCors(AllowClientOrigin);

        if (isDevelopment)
        {
            app.UseCors(AllowScalarOrigin)
                .UseDeveloperExceptionPage();
        }

        return app;
    }
}
