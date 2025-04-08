using System.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using SourceName.Application.ToDos.Contracts;
using SourceName.Infrastructure.Persistence;
using SourceName.Infrastructure.Persistence.PostgreSql;
using SourceName.Infrastructure.Persistence.ToDos;

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
