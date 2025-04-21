using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#if IncludeExample
using SourceName.Application.ToDos.Contracts;
#endif
using SourceName.Infrastructure.Persistence;
using SourceName.Infrastructure.Persistence.PostgreSql;
#if IncludeExample
using SourceName.Infrastructure.Persistence.ToDos;
#endif

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
