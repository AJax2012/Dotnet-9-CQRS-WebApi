using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SourceName.Infrastructure.Persistence;
using SourceName.Infrastructure.Persistence.PostgreSql;

namespace SourceName.Infrastructure.Loaders;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => 
            new NpgsqlConnectionFactory(configuration.GetConnectionString("Default")!));
        
#if IncludeExample
        services.AddScoped<IToDosRepository, ToDosRepository>();
#endif

        return services;
    }
}
